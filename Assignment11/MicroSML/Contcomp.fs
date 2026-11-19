// File MicroSML/Contcomp.fs

//   A compiler from Micro-SML to the abstract machine microVM.

//   Backwards compilation with peephole optimizations corresponding
//   to Chapter 12.

module Comp

open System.IO
open Absyn
open TypeInference
open Machine

let opt_p = CmdLine.chkArg CmdLine.optArg 

// Accumulate code from functions in a mutable list. This makes the
// function cExpr easier to read as the accumulator is not needed

let (resetFuncs,addFunc,getFuncs,
     resetGlobalInit,addGlobalInit,getGlobalInit) =
  let funcInsts : instr list list ref = ref []
  let globalInsts : instr list list ref = ref []
  let reset (iss : instr list list ref) = fun () -> iss.Value <- []
  let add (iss : instr list list ref) lab insts = iss.Value <- (Label lab::insts) :: iss.Value
  let get (iss : instr list list ref) = fun () -> (List.concat << List.rev) iss.Value
  (reset funcInsts,   add funcInsts,   get funcInsts,
   reset globalInsts, add globalInsts, get globalInsts)




type var = 
  | Glovar of int      // A global variable has an absolute address on stack.
  | Locvar of int      // A local variable has an offset relative to base pointer.
  | Closvar of int     // A closure variable has an offset within closure
  | GloTmpvar of int   // A global temporary variable is accessed as an absolute address on
                       // stack - but only for the temporary computation. Relevant for let
                       // expressions part of global expressions.

let ppVar = function
  | Glovar o  -> "Glovar[" + o.ToString() + "]"
  | Locvar o  -> "Locvar[" + o.ToString() + "]"
  | Closvar o -> "Closvar[" + o.ToString() + "]"
  | GloTmpvar o -> "GloTmpvar[" + o.ToString() + "]"

// The variable environment keeps track of global and local variables,
// and keeps track of next available offset for local variables.
type varEnv = var env * int

let incVarIdx (env,idx) i = (env,idx+i)

let ppVarEnv (env,fDepth) =
  "fDepth = " + fDepth.ToString() + "\n" +
  "[ " + (String.concat "\n" (List.map (fun (x,v) -> x + " |-> " + (ppVar v)) env)) + " ]\n"

// Global Exception Number Generator.
//let exnNumVar = "__exnNum__"  
let exnNumVarEnv = ([(exnNumVar,Glovar 0)],1)

// Simple environment operations
type 'data env = (string * 'data) list
let rec lookup env x = 
  match env with 
  | []         -> failwith ("Comp.lookup: " + x + " not found")
  | (y, v)::yr -> if x=y then v else lookup yr x

// No need to include global variables in closures.
// Notice, GloTmpvar are not filtered as they must be included in closure.  
let filterGlobalsInScope (env,_) fvs =
  Set.filter (fun fv -> match lookup env fv with Glovar _ -> true | _ -> false) fvs

// Code-generating functions that perform local optimizations, see
// Chapter 12.

let rec addINCSP m1 C : instr list =
  match (opt_p,C) with
  | (true,INCSP m2            :: c1) -> addINCSP (m1+m2) c1
  | (true,RET m2              :: c1) -> RET (m2-m1) :: c1
  | (true,Label lab :: RET m2 :: _)  -> RET (m2-m1) :: C  // Becomes: RET(m2-m1)::Label lab::RET m2
  | _                                -> if m1=0 then C else INCSP m1 :: C

// Conditional jump to C
let addLabel C : label * instr list = 
  match (opt_p,C) with
  | (true,Label lab :: _) -> (lab, C)
  | (true,GOTO lab :: _)  -> (lab, C)
  | _                     -> let lab = newLabel() 
                             (lab, Label lab :: C)

// Unconditional jump to C
let makeJump C : instr * instr list =          
  match (opt_p,C) with
  | (true,RET m              :: _) -> (RET m, C)
  | (true,Label lab :: RET m :: _) -> (RET m, C)
  | (true,Label lab          :: _) -> (GOTO lab, C)
  | (true,GOTO lab           :: _) -> (GOTO lab, C)
  | _                              -> let lab = newLabel() 
                                      (GOTO lab, Label lab :: C)

// Remove all code until next label
let rec deadcode C = 
  match (opt_p,C) with
  | (true,[])             -> []
  | (true,Label lab :: _) -> C
  | (true,_ :: c1)        -> deadcode c1
  | (false,_)             -> C

let addNOT C =
  match (opt_p,C) with
  | (true,NOT        :: c1) -> c1
  | (true,IFZERO lab :: c1) -> IFNZRO lab :: c1 
  | (true,IFNZRO lab :: c1) -> IFZERO lab :: c1 
  | _                       -> NOT :: C

// jump is GOTO or RET
let addJump jump C =
  if opt_p then  
    let C1 = deadcode C
    match (jump,C1) with
    | (GOTO lab1, Label lab2 :: _) -> if lab1=lab2 then C1 
                                      else GOTO lab1 :: C1
    | _                            -> jump :: C1
  else
    jump :: C                                         

let addGOTO lab C =
  addJump (GOTO lab) C

let rec addCST i C =
  if opt_p then    
    match (i, C) with
    | (0, ADD        :: c1) -> c1
    | (0, SUB        :: c1) -> c1
    | (0, NOT        :: c1) -> addCST 1 c1
    | (_, NOT        :: c1) -> addCST 0 c1
    | (1, MUL        :: c1) -> c1
    | (1, DIV        :: c1) -> c1
    | (0, EQ         :: c1) -> addNOT c1
    | (_, INCSP m    :: c1) -> if m < 0 then addINCSP (m+1) c1
                               else CSTI i :: C
    | (0, IFZERO lab :: c1) -> addGOTO lab c1
    | (_, IFZERO lab :: c1) -> c1
    | (0, IFNZRO lab :: c1) -> c1
    | (_, IFNZRO lab :: c1) -> addGOTO lab c1
    | _                     -> CSTI i :: C
  else    
    CSTI i :: C

// Leave content of variable x at top of the stack
let loadVar varEnv x C : instr list =
  match lookup (fst varEnv) x with
  | Glovar addr  -> addCST addr (LDI :: C)
  | GloTmpvar addr  -> addCST addr (LDI :: C)  
  | Locvar offset  -> GETBP :: addCST offset (ADD :: LDI :: C)
  | Closvar offset -> GETBP :: LDI :: HEAPLDI offset :: C // First access closure and then offset into closure.

// Code for Generative Exception Numbering.
let nextExnNumCode varEnv C =
  match lookup (fst varEnv) exnNumVar with
    // Leave new exception number at top of the stack and update global variable
  | Glovar addr -> addCST addr (addCST addr (LDI :: addCST 1 (ADD :: STI :: C)))
  | _ -> failwith "Contcomp.nextExnNumCode.Global exception variable is not in the environment"
let initExnNumCode addr = addCST addr (addCST 0 [STI])

// Compiling Micro-SML expressions: 
//   * kind    is context for variable, either local or global.
//   * varEnv  is the local and gloval variable environment
//   * e       is the expression to compile
//   * C       is the code following the code for this expression   
let rec cExpr (kind: int->var) (varEnv : varEnv) (e : expr<typ,typescheme>) (C: instr list) : instr list =
  let (env,fdepth) = varEnv
  match e with
  | CstI (i,_) -> addCST i C
  | CstB (b,_) -> addCST (if b then 1 else 0) C
  | CstN _     -> NIL :: C
  | Var (x,_)  -> loadVar varEnv x C
  | Prim1(ope,e1,_) ->
    cExpr kind varEnv e1 
      (match (ope,getTypExpr e1) with
       | ("print",TypI) -> PRINTI :: C
       | ("print",TypB) -> PRINTB :: C
       | ("print",TypL _) -> PRINTL :: C
         // Polymorphic print, prints all scalar values as integers.
       | ("print",t) -> PRINTVAL :: C
       | ("hd",_)    -> CAR :: C  
       | ("tl",_)    ->  CDR :: C
       | ("isnil",_) -> NIL :: EQ :: C
       | _ -> failwith ("cExpr.Prim1 "+ope+" not implemented"))
  | Prim2(ope, e1, e2,_) ->
    cExpr kind varEnv e1
      (cExpr kind (incVarIdx varEnv 1) e2 
        (match ope with
         | "*" -> MUL :: C
         | "+" -> ADD :: C
         | "-" -> SUB :: C
         | "%" -> MOD :: C
         | "=" -> EQ :: C
         | "<>" -> EQ :: addNOT C
         | "<" -> LT :: C
         | ">" -> SWAP :: LT :: C
         | "<=" -> SWAP :: LT :: addNOT C
         | ">=" -> LT :: addNOT C
         | "::" -> CONS :: C
         | _ -> failwith ("cExpr.prim2 " + ope + " not implemented")))
  | AndAlso(e1,e2,_) ->
    match C with
    | IFZERO lab :: _ ->
       cExpr kind varEnv e1 (IFZERO lab :: cExpr kind varEnv e2 C)
    | IFNZRO labthen :: c1 -> 
      let (labelse, c2) = addLabel c1
      cExpr kind varEnv e1
        (IFZERO labelse 
          :: cExpr kind varEnv e2 (IFNZRO labthen :: c2))
    | _ ->
      let (jumpend,  C1) = makeJump C
      let (labfalse, C2) = addLabel (addCST 0 C1)
      cExpr kind varEnv e1
        (IFZERO labfalse 
          :: cExpr kind varEnv e2 (addJump jumpend C2))
  | OrElse(e1,e2,_) ->
    match C with
    | IFNZRO lab :: _ -> 
      cExpr kind varEnv e1 (IFNZRO lab :: cExpr kind varEnv e2 C)
    | IFZERO labthen :: c1 ->
      let(labelse, c2) = addLabel c1
      cExpr kind varEnv e1 
        (IFNZRO labelse :: cExpr kind varEnv e2
          (IFZERO labthen :: c2))
    | _ ->
      let (jumpend, C1) = makeJump C
      let (labtrue, C2) = addLabel(addCST 1 C1)
      cExpr kind varEnv e1
        (IFNZRO labtrue :: cExpr kind varEnv e2 (addJump jumpend C2))
  | Seq(e1,e2,_) ->
    cExpr kind varEnv e1 
      (addINCSP -1 // Remove result of e1
        (cExpr kind varEnv e2 C))
  | Let(valdecs,letBody) ->
    let ((_,fdepth') as bodyEnv,vdEnvs) =
      List.fold (fun (accEnv,accVdEnv) vd -> let (nextEnv,vdEnv) = genValdecEnv kind accEnv vd
                                             (nextEnv,(vd,vdEnv)::accVdEnv)) (varEnv,[]) valdecs 
    let vdEnvs' = List.rev vdEnvs
    let numVals = fdepth' - fdepth
    let iVals C = List.foldBack (cValdec kind) vdEnvs' C
    let addrFirstVal C = GETSP :: addCST (numVals - 1) (SUB :: C)
    let iBody C = cExpr kind (incVarIdx bodyEnv 1) letBody C // Make room for addrFirstVal.
    iVals (addrFirstVal (iBody (STI :: addINCSP -numVals C)))
  | If(e1, e2, e3) ->
    let (jumpend, C1) = makeJump C
    let (labelse, C2) = addLabel (cExpr kind varEnv e3 C1)
    cExpr kind varEnv e1 (IFZERO labelse
      :: cExpr kind varEnv e2 (addJump jumpend C2))                        
  | Fun(x,fBody,_) ->
    let funcLab = newLabel()
    // To minimize closures, we do not copy globals in scope in closure.
    let fvsAll = freevars fBody - (set [x]) 
    let fvsGlobalInScope = filterGlobalsInScope varEnv fvsAll
    let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
    let _ = CmdLine.debug ("FN " + funcLab + ", parameter " + x + ":\n" +
                           "  fvsAll: " + (ppFreevars fvsAll) + 
                           "  fvsGlobalsInScope: " + (ppFreevars fvsGlobalInScope) + 
                           "  fvs in clos: " + (ppFreevars fvsClos) +
                             "  varEnv: " + (ppVarEnv varEnv)) 
    // Closure at index 0; argument at index 1; fv1 at index 1 in closure; idx 0 is code pointer.
    let bodyEnv = (x, Locvar 1) ::
                  (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                  (List.map (fun x -> (x,lookup (fst varEnv) x)) (Set.toList fvsGlobalInScope))
    // Add function to global program. Closure/Arg at index 0/1
    let _ = addFunc funcLab (cExpr Locvar (bodyEnv,2) fBody [RET 2])
    let sizeClos = List.length fvsClos + 1
    let codeFreevars' = List.foldBack (loadVar varEnv) fvsClos (HEAPALLOC (Machine.closTag,sizeClos) :: HEAPCOPY sizeClos :: C)  
    PUSHLAB funcLab :: codeFreevars'
  | Call(eFun, eArg,tOpt,_) ->
    let cInst C = match (opt_p,tOpt) with (true,Some true) -> TCLOSCALL 1 :: deadcode C | _ -> CLOSCALL 1 :: C
    cExpr kind varEnv eFun (cExpr kind (incVarIdx varEnv 1) eArg (cInst C))
  | Raise(e,_) -> cExpr kind varEnv e (THROW :: deadcode C) 
  | TryWith(e1,ExnVar exn,e2) -> // Jump optimization left as exercise.
    let labend = newLabelWName "TryWithEnd"
    let labexn = newLabelWName ("TryWith_" + exn)
    loadVar varEnv exn
      (PUSHHDLR labexn ::
        (cExpr kind (incVarIdx varEnv 3 (* Handler size = 3 *)) e1 
          (POPHDLR :: GOTO labend :: Label labexn ::
            (cExpr kind varEnv e2 (Label labend :: C)))))

// genValdecEnv returns two environments (nextEnv,vdEnv):
//   - nextEnv is the environment to be used by the following vd or body
//   - vdEnv is the environment to be used by the vd at hand.
// Notice the difference. For Fundecs nextEnv and vdEnv is the same because
// all functions in the group can be used in the body for each function.
// This is NOT the case for let bound variables where the variable is not in
// scope until the following vd or body.
and genValdecEnv (kind: int->var) ((env,fdepth) as curEnv) vd =
  match vd with        
  | Fundecs fs ->
    let newEnv = List.fold (fun (env,fdepth) (f,x,fBody,_) ->
                             ((f,kind fdepth) :: env, fdepth+1)) curEnv fs
    (newEnv,newEnv)
  | Valdec (x,eRhs,_) -> (((x,kind fdepth) :: env, fdepth+1),curEnv)
  | Exndec(ExnVar exn) -> (((exn,kind fdepth) :: env, fdepth+1),curEnv)
  
and cValdec (kind: int->var) (vd:valdec<typ,typescheme>, varEnv: varEnv) (C: instr list) : instr list =
  // varEnv has been precalculated to be the environment for the valdec to compile
  match vd with
  | Fundecs fs ->
    // Calculate fvs for each function in fs.
    let fsfvs =
      List.map (fun (f,x,fBody,_) ->
                  // To minimize closures, do not copy globals in scope in closure.
                  let fvsAll = freevars fBody - (set [x;f]) 
                  let fvsGlobalInScope = filterGlobalsInScope varEnv fvsAll
                  let fvsClos = Set.toList (fvsAll - fvsGlobalInScope)
                  CmdLine.debug ("Fundecs: "+ f + ", parameter " + x + ":\n" +
                                 "  fvsAll: " + (ppFreevars fvsAll) +
                                 "  fvsGlobalsInScope: " + (ppFreevars fvsGlobalInScope) +         
                                 "  fvs in clos: "+(ppFreevars fvsClos) +
                                 "  varEnv: " + (ppVarEnv varEnv))
                  let labFunc = newLabelWName ("LabFunc_" + f)
                  (f,x,fBody,fvsGlobalInScope,fvsClos,List.length fvsClos + 1,labFunc)) fs
    // Code to allocate closures.
    let iaClos C = List.foldBack (fun (_,_,_,_,_,sizeClos,_) C -> HEAPALLOC (Machine.closTag,sizeClos) :: C) fsfvs C
    // Code to copy free variables and code label to each closure.
    let iFillClos C =
      List.foldBack (fun (f,_,_,_,fvsClos,sizeClos,funcLab) C ->
                       let codefvsClos C = List.foldBack (loadVar varEnv) fvsClos C
                       let codeClosPtr C = loadVar varEnv f C
                       PUSHLAB funcLab :: codefvsClos (codeClosPtr (HEAPCOPY sizeClos :: addINCSP -1 C))) fsfvs C
    // Generate code for each function body.
    let codefBody (f,x,fBody,fvsGlobalInScope,fvsClos,_,funcLab) =
      // Closure at index 0, argument at index 1, fv1 at index 1 in closure; idx 0 is code pointer.
      let varEnvBody = (f, Locvar 0) :: (x, Locvar 1) ::
                       (List.mapi (fun i x -> (x,Closvar (i+1))) fvsClos) @
                        (List.map (fun x -> (x,lookup (fst varEnv) x)) (Set.toList fvsGlobalInScope))
      addFunc funcLab (cExpr Locvar (varEnvBody,2) fBody [RET 2])
    List.iter codefBody fsfvs
    let insts = iaClos (iFillClos C)
    insts

  | Valdec (x,eRhs,_) ->
    CmdLine.debug ("Valdec "+ x + ":\n" +
                   "  varEnv: " + (ppVarEnv varEnv))
    cExpr kind varEnv eRhs C

  | Exndec(ExnVar exn) ->
    CmdLine.debug ("Exndec "+ exn + ":\n" +
                   "  varEnv: " + (ppVarEnv varEnv))
    
    // Code to push next exn number on stack.
    nextExnNumCode varEnv C

and cProgram (p:program<typ,typescheme>) : instr list =
  let _ = resetLabels()
  let _ = resetFuncs()
  let _ = resetGlobalInit()
  let labMain = newLabel()
  // Global exception number as first global variable, addr 0.
  // See exn07.sml and exn08.sml  
  let initEnv = exnNumVarEnv 
  let _ = addGlobalInit (newLabelWName "G_ExnVar") (initExnNumCode 0) 
  match p with
    | Prog(valdecs,e) ->
    let ((bodyEnv,fdepth'),vdEnvs) =
      List.fold (fun (accEnv,accVdEnv) vd -> let (nextEnv,vdEnv) = genValdecEnv Glovar accEnv vd
                                             (nextEnv,(vd,vdEnv)::accVdEnv)) (initEnv,[]) valdecs
    let vdEnvs' = List.rev vdEnvs
    let numVals = fdepth'   // Remove everything below the end result value.
    // It is wrong to use Glovar instead of GloTmpvar
    // Try and compile ex17.sml.
    let iVals C = List.foldBack (cValdec GloTmpvar) vdEnvs' C
    let _ = addGlobalInit (newLabelWName "G_Valdecs") (iVals [])
    let _ = addFunc labMain ((cExpr Locvar (bodyEnv,0) e) [RET 0])
    getGlobalInit() @
    (GETSP :: addCST (numVals-1) (SUB :: CALL(0,labMain) :: STI :: addINCSP -numVals (STOP :: getFuncs())))
    
// Compile a complete micro-SML program and write the resulting
// instruction list to file fname; also, return the program as a list
// of instructions.

let intsToFile (inss : int list) (fname : string) = 
  File.WriteAllText(fname, String.concat " " (List.map string inss))

