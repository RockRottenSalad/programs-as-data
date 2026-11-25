(* File MicroJ/Contcomp.fs

   A compiler from Micro-Java to an abstract machine.

   Backwards compilation with peephole optimizations corresponding to
   Chapter XX.

*)

module Comp

open System.IO
open Util
open Absyn
open Machine

(* ------------------------------------------------------------------- *)

(* Code generator creates class descriptors containing code before     *)
(* flattening everything to one long instr list.                       *)

type classRepr = {
  classLab  : label;          // Label for class descriptor
  className : string;         // Name of class.
  superLab  : label;          // Label for super class descriptor, empty string for root.
  vTable    : label list;     // Labels for virtual method table
  methods   : instr list list // Bytecode for methods, label is first instruction in list.
}

(* ------------------------------------------------------------------- *)

(* The intermediate representation between passes 1 and 2 above:  *)

type bstmtordec =
     | BDec of instr list                  (* Declaration of local variable  *)
     | BStmt of stmt                       (* A statement                    *)

(* ------------------------------------------------------------------- *)

(* Environment operations *)
type env<'key,'data> when 'key: comparison = Map<'key,'data>

let emptyEnv = Map.empty
let tryLookup env x = Map.tryFind x env

let lookup env x =
  match tryLookup env x with
    None -> fatal (sprintf "lookup: key %s not found in env %A" (x.ToString()) (env.ToString()))
  | Some v -> v

// (cn,fld) is the class where fld is accessed - but fld may not be defined
// in cn - it may be defined in a super class why we need to go up the hierarchy chain.
let lookupFld classEnv (cn,fld) =
  let rec loop cn' =
    let ((fldEnv,_),_,_,supern) = lookup classEnv cn'
    match tryLookup fldEnv (cn',fld) with
      Some v -> v
    | None when supern <> "" -> loop supern
    | None -> fatal (sprintf "lookupFld: key %s, class %s, not found in chain up to root %A" fld cn (classEnv.ToString()))
  loop cn
  
// We can't have duplicates in vTable due to dynamic dispatch.
// Reason for representing environments as maps.
// vTable, see vTableLabs in cClassDec.
let addEnv k v env = Map.add k v env

let ppEnv fnPP env =
  String.concat nl (List.map fnPP (Map.toList env))

type var = int // Local variable has address relative to bottom of frame. There are no global variables.

type varEnv = env<string,var>*int                           // Local variable name -> offset in stack frame and next offset.
type fldEnv = env<string*string,int*typ>*int                // Class and field name -> offset into runtime object, type and next offset.
type vTableEnv = env<methodSignature,int*label> * int       // Method signature -> offset into vTable and label and next offset
type classEnv  = env<string,fldEnv*vTableEnv*label*string>  // Class name -> field and vTable environment,
                                                            // code label of class descriptor and name of super class.

(* Pretty Print environments *)
let ppFldEnv (env,offset) =
  sprintf "  Field Environment with next offset %d:" offset + nl +
  (ppEnv (fun ((cn,fldn),(o,typ)) -> sprintf "    %s.%s:%s -> %d" cn fldn (ppTyp typ) o) env)

let ppvTableEnv (env,offset) =
  sprintf "  vTable Environment with next offset %d:" offset + nl +
  (ppEnv (fun (mthSig,(o,mthLab)) -> sprintf "    %s -> (%d,%s)" (ppMthSig mthSig) o mthLab) env)

let ppClassEnv classEnv =
  "Class Environment: " + nl +
  (ppEnv (fun (cn,(fldEnv,vTableEnv,classLab,supern)) -> "  " + cn + " (" + classLab + "): " + supern + ":" + nl + (ppFldEnv fldEnv)
                                                         + nl + (ppvTableEnv vTableEnv)) classEnv)
// Build field environment
// Assign field next offset in field environment
let addToFldEnv cn ((env,offset) as fldEnv) = function
    Methoddec _ -> fldEnv
  | Fielddec(ty,fld,_) -> (addEnv (cn,fld) (offset,ty) env,offset+1)

let addTovTableEnv ((env,offset) as vTableEnv) = function
    Methoddec (_,mName,_,_,_) as md ->
      let mSig = mthSig md
      match tryLookup env mSig with
        None -> let mLab = newLabel() + "_" + mName        // Method does not exists, new label.
                (addEnv mSig (offset,mLab) env,offset+1)
      | Some (o,_) -> let mLab = newLabel() + "_" + mName  // Re-use slot but new label, overrided method.
                      (addEnv mSig (o,mLab) env,offset)   
  | Fielddec _ -> vTableEnv

let buildClassEnv (ch:classHierarchy) : classEnv =
  let buildEnvClassDec classEnv (Classdec(cn,supern,mds,_)) =
    let (fldEnv0,vTableEnv0,_,_) = lookup classEnv supern
    let fldEnv = List.fold (addToFldEnv cn) fldEnv0 mds
    let vTableEnv = List.fold addTovTableEnv vTableEnv0 mds
    let classLab = newLabel() + "_class_" + cn
    addEnv cn (fldEnv,vTableEnv,classLab,supern) classEnv
  match ch with
    Hierarchy(Classdec("Object","",[],_), chs) ->
      // First field index is 1 - 0 is for class description pointer.
      let classEnv0 = addEnv "Object" ((emptyEnv,1),(emptyEnv,0),newLabel() + "_class_Object", "") emptyEnv
      List.fold (fun classEnv ch' ->
                 foldCH buildEnvClassDec classEnv ch') classEnv0 chs
  | Hierarchy _ -> fatal "Contcomp.buildClassEnv."  

// Bind declared variable in varEnv and generate code to allocate it:
let allocate (typ, x) ((env, fdepth) : varEnv) : varEnv * instr list =
    match typ with
    | TypB | TypI | TypN | TypO _ -> 
      let newEnv = (addEnv x fdepth env, fdepth+1)
      let code = if (typ = TypB || typ = TypI) then [CSTI 0] else [NIL]
      (newEnv, code)
    | TypS -> fatal "Contcomp.allocate: Strings not implemented"
    | TypV -> fatal "Contcomp.allocate: Can't allocate void type."    

// Bind declared parameter in local variable env.
let bindParam (env, fdepth) (typ, x) : varEnv = 
    (addEnv x fdepth env, fdepth+1)
let bindParams paras (env, fdepth) : varEnv = 
    List.fold bindParam (env, fdepth) paras;

let cConstant c C =
  match c with
    CstI i -> CSTI i :: C
  | CstB b -> (if b then CSTI 1 else CSTI 0) :: C
  | CstS s -> fatal "Contcomp.cConstant, String not implemented."
  | CstN  -> NIL :: C

let rec cExpr (e: expr) (varEnv: varEnv) (classEnv : classEnv) (C: instr list) : instr list =
  match e with
  | Access a ->
      cAccess a varEnv classEnv (LDD :: C) 
  | Assign (a,e,pos) ->
      cAccess a varEnv classEnv (cExpr e varEnv classEnv (STD :: C))  
  | Cst(c,aOpt,pos) -> cConstant c C
  | New(cn,aOpt,pos) -> 
      let ((fldEnv,_),_,classLab,_) = lookup classEnv cn
      // Generate initialization instructions for fields - sort by offset
      let genInitInstrTyp = function
          TypB | TypI -> CSTI 0
        | TypN | TypO _ -> NIL
        | TypS -> fatal "Contcomp.cClassDec.getInitCodeTyp: Strings not implemented"
        | TypV -> fatal "Contcomp.cClassDec.getInitCodeTyp: Can't allocate void type."    
      let fieldsInitTyps = List.map snd (List.sortBy fst (List.map snd (Map.toList fldEnv)))
      let codeFieldsInit = List.map genInitInstrTyp fieldsInitTyps
      let sizeObj = List.length fieldsInitTyps + 1   // +1 for classLab.
      PUSHLAB classLab :: codeFieldsInit @ (HEAPALLOC (Machine.objectTag,sizeObj) :: HEAPCOPY sizeObj :: C)
  | Prim1(ope,e1,aOpt,pos) ->
      cExpr e1 varEnv classEnv
        (match ope with
         | "!" -> NOT :: C
         | _        -> fatal ("Contcomp.cExpr.Prim1, Panic - unknown primitive " + ope))
  | Prim2(ope,e1,e2,aOpt,pos) -> 
      cExpr e1 varEnv classEnv
        (cExpr e2 varEnv classEnv
           (match ope with
            | "*"   -> MUL  :: C
            | "+"   -> ADD  :: C
            | "-"   -> SUB  :: C
            | "/"   -> DIV  :: C
            | "%"   -> MOD  :: C
            | "=="  -> EQ   :: C
            | "!="  -> EQ   :: NOT :: C
            | "<"   -> LT   :: C
            | ">="  -> LT   :: NOT :: C
            | ">"   -> SWAP :: LT :: C
            | "<="  -> SWAP :: LT :: NOT :: C
            | _     -> fatal ("Contcomp.cExpr.Prim2 - unknown primitive " + ope)))
  | PrimC(ope,es,typOpt,pos) -> 
    match ope with
      "print" | "println" ->
        let C = // Maybe add extra PRINTNL at end.
          match ope with
            "print" -> C
          | "println" -> PRINTNL :: C
          | _ -> fatal ("Contcomp.cExpr.PrimC on ope " + ope + " not implemented.")
        let genPrintInstr typ C =
          match typ with
            TypI   -> PRINTI :: C
          | TypB   -> PRINTB :: C
          | TypN   -> PRINTN :: C
          | TypO _ -> PRINTO :: C
          | typ    -> fatal ("Contcomp.cExpr.PrimC.Print - type " + (ppTyp typ) + " not supported by print")
        let genPrintNotLast e C = // removes result from stack
          cExpr e varEnv classEnv (genPrintInstr (getTypExpr e) (INCSP -1 :: C))
        let genPrintLast e C =  // leaves result on stack.
          cExpr e varEnv classEnv (genPrintInstr (getTypExpr e) C)
        let C =
          match List.length es with
            0 -> CSTI 1 :: C   // No arguments - just leave 1 on stack.
          | numEs -> let (es',laste) = (List.take (numEs-1) es, List.last es)
                     List.foldBack genPrintNotLast es' (genPrintLast laste C)
        C
    | _ -> fatal ("Contcomp.cExpr.PrimC on ope " + ope + " not implemented.")
  | Andalso(e1,e2,aOpt,pos) ->
      let labend   = newLabel()
      let labfalse = newLabel()
      cExpr e1 varEnv classEnv
        (IFZERO labfalse ::
          (cExpr e2 varEnv classEnv (GOTO labend :: Label labfalse :: CSTI 0 :: Label labend :: C)))
  | Orelse(e1, e2,aOpt,pos) ->
      let labend  = newLabel()
      let labtrue = newLabel()
      cExpr e1 varEnv classEnv
        (IFNZRO labtrue ::
          (cExpr e2 varEnv classEnv (GOTO labend :: Label labtrue :: CSTI 1 :: Label labend :: C)))
  | Call(Access(AccVar("super",typOpt,_)),mName,sigOpt,es,aOpt,pos) -> // Compile time resolved method call to super.mName
      let superTyp = Type.getClassName (typOpt.Value) // Will not fail due to type check.
      let (_,(vTable,_),_,_) = lookup classEnv superTyp
      let vSig = sigOpt.Value // Will not fail after type check.
      let argc = List.length es + 1 // this must be first argument - works also for super.
      if argc <> List.length (snd vSig) + 1
        then fatal ("Contcomp.cExpr.Call, super." + mName + " has parameter/argument mismatch")
      let (_,mLab) = lookup vTable vSig   // Get static label of method to call in super type.
      cExpr (Access(AccVar("this",None,emptyPos))) varEnv classEnv   // Use "this" as current object.
        (cExprs es varEnv classEnv (CALL(argc, mLab) :: C))  // Static CALL instruction.
  | Call(eObj,mName,vSigOpt,es,aOpt,pos) -> // Dynamic dispatch
      let classType = getClassTypExpr eObj
      let (_,(vTable,_),_,_) = lookup classEnv classType
      let vSig = vSigOpt.Value // Will not fail after type check.
      let argc = List.length es + 1 // this is first argument
      if argc <> List.length (snd vSig) + 1
        then fatal ("Contcomp.cExpr.Call, " + mName + " has parameter/argument mismatch")
      let (offset,_) = lookup vTable vSig
      cExpr eObj varEnv classEnv  // eObj is extra argument, this.
        (cExprs es varEnv classEnv (VCALL(argc, offset) :: C))

and cExprs es varEnv classEnv C =
  match es with
    [] -> C
  | e::er -> cExpr e varEnv classEnv (cExprs er varEnv classEnv C)

and cAccess a varEnv classEnv C =
  match a with
    AccVar(x,_,pos) ->
      match lookup (fst varEnv) x with
        offset -> GETBP :: CSTI offset :: ADD :: STACKADDR :: C
  | AccFld(Access(AccVar("super",typOpt,_)),fld,_,pos) ->
      let superTyp = Type.getClassName (typOpt.Value) // Will not fail due to type check.
      // Offset for field in super class is the same as in current object.
      match lookupFld classEnv (superTyp,fld) with
                      // Current object is "this", in which super.f exists.
        (offset,_) -> cExpr (Access(AccVar("this",None,emptyPos))) varEnv classEnv (CSTI offset :: HEAPADDR :: C)
  | AccFld(e,fld,_,pos) ->
    let classType = getClassTypExpr e
    // Offset for field in eObj class is the same as in current object.
    match lookupFld classEnv (classType,fld) with  
      (offset,_) -> cExpr e varEnv classEnv (CSTI offset :: HEAPADDR :: C)
      
and cStmt stmt (varEnv: varEnv) (classEnv: classEnv) (C: instr list) : instr list =
  match stmt with
  | If(e,stmt1,stmt2,_) ->
    let labelse = newLabel()
    let labend  = newLabel()
    cExpr e varEnv classEnv (IFZERO labelse ::
                              (cStmt stmt1 varEnv classEnv
                                (GOTO labend :: Label labelse ::
                                  (cStmt stmt2 varEnv classEnv (Label labend :: C)))))
  | While(e,body,_) -> 
    let labbegin = newLabel()
    let labtest  = newLabel()
    GOTO labtest :: Label labbegin ::
      (cStmt body varEnv classEnv (Label labtest :: cExpr e varEnv classEnv (IFNZRO labbegin :: C)))
  | Expr e ->
    cExpr e varEnv classEnv (INCSP -1 :: C) 
  | Return(None,_) ->
    RET (snd varEnv - 1) :: C   // -1 because there are no result value on the stack - leave dummy one.
  | Return (Some e,_) ->
    cExpr e varEnv classEnv (RET (snd varEnv) :: C)
  | Block(stmtOrDecs,_) ->
    let rec pass1 stmtOrDecs ((_, fdepth) as varEnv) =
      match stmtOrDecs with 
      | []     -> ([], fdepth)
      | s1::sr ->
        let (_, varEnv1) as res1 = bStmtOrDec s1 varEnv
        let (resr, fdepthr) = pass1 sr varEnv1 
        (res1 :: resr, fdepthr) 
    let (stmtsback, fdepthend) = pass1 stmtOrDecs varEnv
    let rec pass2 pairs C = 
      match pairs with 
      | [] -> C
      | (BDec code,  varEnv) :: sr -> code @ pass2 sr C
      | (BStmt stmt, varEnv) :: sr -> cStmt stmt varEnv classEnv (pass2 sr C)
    pass2 stmtsback (INCSP (snd varEnv - fdepthend) :: C) // Remove variables, declared in the block, from the stack

and bStmtOrDec stmtOrDec varEnv : bstmtordec * varEnv =
    match stmtOrDec with 
    | Stmt stmt    ->
      (BStmt stmt, varEnv) 
    | Dec (typ,x,_) ->
      let (varEnv1, code) = allocate (typ, x) varEnv 
      (BDec code, varEnv1)

and cMemberDec classn classEnv (memberDec: memberdec) : instr list =
    match memberDec with
    | Methoddec(ty, mName, paras, body, _) ->
      // Get label of method to compile - will not fail due to type check.
      let (_,(vTableEnv,_),_,_) = lookup classEnv classn
      let (_,mthLab) = lookup vTableEnv (mthSig memberDec)
      let varEnv0 = addEnv "this" 0 emptyEnv
      let varEnv = bindParams paras (varEnv0,1)
      let code = cStmt body varEnv classEnv []
      Label mthLab :: code
    | Fielddec(ty,fld,_) -> fatal "Contcomp.cMemberDec.Fielddec - should never get here."

and cClassDec classEnv (Classdec (classn, supern, memberdecs, _)) : classRepr =

  // Get label of class to compile - will not fail due to type check.
  let (_,(vTableEnv,_),classLab,_) = lookup classEnv classn 

  // Get label of super class unless Object.
  let superLab =
    match classn with
      "Object" -> ""
    | _ -> let (_,_,classLab,_) = lookup classEnv supern
           classLab
    
  // Generate code for methods
  let mthDecs = List.filter isMethod memberdecs
  let mthCodes = List.map (cMemberDec classn classEnv) mthDecs

  // Generate labels to methods based on vTable - sort by offset This
  // code depends on keys, that is method signatures, are unique in
  // the env. Therefore the remove in function addEnv.
  let vTableLabs = List.map snd (List.sortBy fst (List.map snd (Map.toList vTableEnv)))

  { classLab = classLab;    
    className = classn;
    superLab = superLab; 
    vTable = vTableLabs;
    methods = mthCodes } 

and cClassHierarchy (classEnv: classEnv) (Hierarchy(Classdec(classn,_,_,_) as classdec, chs) as ch) : classRepr list =
  let classReprs = foldCH (fun classReprs cd -> cClassDec classEnv cd :: classReprs) [] ch
  classReprs

let flattenCR (initCode: instr list) (classReprs: classRepr list) : instr list =
  let flattenClass { classLab = cl; className = _; superLab = _;
                     vTable = vt; methods = mths } C =
    let mthsCode = List.concat mths @ C
    Label cl ::
      (List.foldBack (fun lab C -> LabelAddr lab :: C) vt mthsCode)
  initCode @ (List.foldBack flattenClass classReprs [])

let cProg (cmdL: CmdLine.cmdLine) (ch:classHierarchy) : instr list =
  CmdLine.verbose ("Compiling file " + cmdL.source)
  let _ = resetLabels()

  // Build class environment - field index into object and method index into vTable - for each class.
  let classEnv = buildClassEnv ch
  CmdLine.verbose (ppClassEnv classEnv)

  // Compile class hierarchy using class environment.
  let classReprs = cClassHierarchy classEnv ch

  // Locate main method to call
  let (_,(vTableMain,_),classLab,_) = lookup classEnv "Main"
  let (mainIndex,mainLab,mainTyps) =
    match Map.tryPick (fun (n,typs) (o,lab) -> if n = "main" then Some (o,lab,typs) else None) vTableMain with
      Some o_lab_typs -> o_lab_typs
    | None -> fatal "Contcomp.mainLab - never happens due to type check"
  let mainArgc = List.length mainTyps
  CmdLine.debug ("Main method with label " + mainLab + " found in class " + classLab +
                 " with vTable index " + (mainIndex.ToString()))

  let prgInit =
    cExpr (New("Main",Some (TypO "Main"), emptyPos)) (emptyEnv,0) classEnv
      [LDARGS mainArgc; VCALL(mainArgc+1, mainIndex); STOP]  // +1, for mandatory this argument

  // Flatten machine program into a list of instructions.
  let flattenInstrs = flattenCR prgInit classReprs
  flattenInstrs
