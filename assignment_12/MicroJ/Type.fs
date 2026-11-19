// Typing micro--Java program
module Type

open Util
open Absyn

// Environment operations
type env<'k,'v> = List<'k * 'v>

let rec lookup env x = 
    match env with 
    | []         -> failwith ("Type error: Key " + x.ToString() +
                              " not found in environment " + env.ToString() + ".")
    | (y, v)::yr -> if x=y then v else lookup yr x

let inDom env x = List.exists (fun (y,_) -> x=y) env

let ppEnv fnPP env =
  String.concat nl (List.map fnPP env)

// Variable type environment, Gamma, used for method parameters and local variables
type varEnv = env<string,typ>

// Field type environment, Phi, used for class fields
type fldEnv = env<string,typ>

// Method type envrionment, Psi, mapping method signatures to method types.
type mthEnv = env<methodSignature,methodType>

// Class table maps class names to field and method type environment.
type ctEnv = env<string,mthEnv*fldEnv>

// Pretty Print Environments
let ppVarEnv = ppEnv (fun (x,t) -> x + ": " + (ppTyp t))
let ppFldEnv = ppEnv (fun (f,t) -> f + ": " + (ppTyp t))
let ppMthEnv = ppEnv (fun (mthSig,mthTyp) -> ppMthSig mthSig + ": " + (ppMthTyp mthTyp))
let ppCtEnv = ppEnv (fun (cn,(mthEnv,fldEnv)) -> "Class Table for " + cn + ": " + nl +
                                                        (ppMthEnv mthEnv) + nl + (ppFldEnv fldEnv))

// Helper functions - other helper functions are in Absyn.fs.
let allDistinct xs = List.length xs = List.length (List.distinct xs)

let typeError pos errMsg = fatal ("Type error on line " + (string)pos.line + ", column " +
                                 (string)pos.column + ": " + nl + "  " + errMsg)

let chkPremise pos p errMsg = if not (p()) then typeError pos errMsg
  
let isScalarType = function
    TypI -> true
  | TypB -> true
  | _ -> false

let isReferenceType = function
    TypS -> true
  | _ -> false
  
let isBuiltInType t = isScalarType t (* || isReferenceType t Exercise *)

let isClassType ct = function  
  | TypO cn -> inDom ct cn
  | TypN -> true
  | _ -> false

// Combination of types where we implement equality.
let areEqualityTypes = function
    (TypI,TypI) -> true
  | (TypS,TypS) -> true
  | (TypB,TypB) -> true
  | (TypO _, TypO _) -> true
  | (TypN, TypO _) -> true
  | (TypO _, TypN) -> true
  | (TypN, TypN) -> true
  | (_,_) -> false

// Combination of types where we implement an ordering, only scalar values.
let areOrderedTypes = function
    (TypI,TypI) -> true
  | (TypB,TypB) -> true    
  | (_,_) -> false

// Class type must be declared and not null.
let isDeclaredClassType ct = function  
  | TypO cn -> inDom ct cn
  | TypN -> false
  | _ -> false

let getClassName = function
  | TypO cn -> cn
  | _ -> fatal "Type.getClassName: Type is not a class type."  // Should never happen.

let isMthResType ct = function
    TypV -> true  // Return type void is ok.
  | t -> isBuiltInType t || isDeclaredClassType ct t  // Return type can't be null.

// Returns true if t1 is a subtype to t2, t1 <: t2
let rec isSubType ch t1 t2 =
  match (t1,t2) with
    (TypI,TypI) -> true
  | (TypB,TypB) -> true
  | (TypS,TypS) -> true
  | (TypN,TypN) -> true
  | (TypN,TypO _) -> true    
  | (TypO cn1,TypO cn2) when cn1 = cn2 -> true
  | (TypO "Object", _) -> false // Makes sure the search below does not fail on super to Object class.
  | (TypO cn1,TypO cn2) ->
    match tryFindClass ch cn1 with
      None -> fatal ("Type.isSubType: can't find class type " + cn1 + " in class hierarchy " +
                     (ppClassHierarchy ch) + ".")
    | Some (Classdec(_,super,_,_)) -> isSubType ch (TypO super) t2
  | (TypV,_) -> fatal ("Type.isSubType: void type should never be used in context of a declared type.")
  | (_,TypV) -> fatal ("Type.isSubType: void type should never be used in context of a declared type.")
  | _ -> false

// Subtyping including void methods, <:_lambda
let isResSubType ch t1 t2 =
  match (t1,t2) with
    (TypV,TypV) -> true  // Return types of two void methods also fulfil subtype relation.
  | _ -> isSubType ch t1 t2

// More specific method signature
let isMthMoreSpecific ch (m1,typs1) (m2,typs2) =
  m1 = m2 &&
  List.length typs1 = List.length typs2 &&
  List.forall (fun (t1,t2) -> isSubType ch t1 t2) (List.zip typs1 typs2)

// Build Class Table

// Rule: CT-Class
let buildCTClass ch ct (Classdec(cn,supern,mds,pos)) =
  let flds = List.filter isField mds
  let fldns = List.map fldName flds
  let mths = List.filter isMethod mds
  let mthSigs = List.map mthSig mths
  let (mthEnv,fldEnv) = lookup ct supern
  chkPremise pos (fun () -> allDistinct fldns)  // exF10.java
             ("Field names are not distinct in class " + cn + ": " + (String.concat ", " fldns) + ", (CT-Class).")
  chkPremise pos (fun () -> allDistinct mthSigs) // exF11.java
             ("Method signatures are not distinct in class " + cn + ": " +
              (String.concat ", " (List.map ppMthSig mthSigs)) + ", (CT-Class)")
  let chkMth mth = chkPremise pos (fun () -> (not (inDom mthEnv (mthSig mth))) ||  // exF12.java
                                             isResSubType ch (mthResType mth) (snd (lookup mthEnv (mthSig mth))))
                              ("Method " + (ppMthSig (mthSig mth)) + " in class " + cn +
                               " cannot override previously declared method because result type " + (ppTyp (mthResType mth)) +
                               " is not a subtype of result type of method to override, (CT-Class).")
  List.iter chkMth mths
  (List.fold (fun env mth -> (mthSig mth, mthTyp mth) :: env) mthEnv mths,
   List.fold (fun env fld -> (fldName fld, fldTyp fld) :: env) fldEnv flds)

// Rule: CT-Prog
let buildCT (ch:classHierarchy) : ctEnv =
  let cns = foldCH (fun e cd -> className cd::e) [] ch
  chkPremise emptyPos (fun () -> not(List.contains "super" cns ||
                                     List.contains "this" cns)) // exF53, exF54
             ("The class names this or super are not allowed, (CT-Prog).")
  chkPremise emptyPos (fun () -> allDistinct cns) // exF01.java
             ("Class names are not distinct: " + (String.concat ", " cns) + ", (CT-Prog).")
  let ct0 = [("Object",([],[]))] // Initial class table containing empty Object class.
  match ch with
    Hierarchy(Classdec("Object","",[],_), chs) ->  // Object must be class at top of hierarchy.
      List.fold (fun ct ch' -> 
                   foldCH (fun ct cd -> (className cd, buildCTClass ch ct cd) :: ct) ct ch') ct0 chs
    // Below can't happen as root Object is explicitly aded in Absyn.buildClassHierarchy.
  | Hierarchy _ -> typeError emptyPos "Type error in program: Top class in hierarchy is not Object, (CT-Prog)."

// Typing Expressions
                 
let rec typExpr (ch:classHierarchy) (ct:ctEnv) (varEnv:varEnv) (e:expr) : expr*typ =
  let ppExpr = ppExpr false false 0  // Partial apply on ppExpr in Absyn
  match e with
      Cst(c,_,pos) ->  // Rule E-Int, E-Null, E-True, E-False
        (match c with
           CstI _ -> (Cst(c,Some TypI,pos),TypI)
         | CstS _ -> (Cst(c,Some TypS,pos),TypS)
         | CstB _ -> (Cst(c,Some TypB,pos),TypB)
         | CstN   -> (Cst(c,Some TypN,pos),TypN))
    | Access a -> // Rule E-Access
        let (a',aTyp) = typAccess ch ct varEnv a
        (Access a',aTyp)  
    | Assign (a,e,pos) -> // Rule E-Assign
        let (a',aTyp) = typAccess ch ct varEnv a  
        let (e',eTyp) = typExpr ch ct varEnv e
        chkPremise pos (fun () -> isSubType ch eTyp aTyp) // exF26.java
               ("Expression " + (ppExpr e) + " of type " + (ppTyp eTyp) + " is not a subtype of " +
                (ppTyp aTyp) + ", (E-Assign).")
        (Assign(a',e',pos),aTyp) 
    | Prim1(ope,e,_,pos) -> // Rule E-Not
        let (e',eTyp) = typExpr ch ct varEnv e
        match (ope,eTyp) with
          ("!",TypB) -> (Prim1(ope,e',Some TypB,pos),TypB)
        | _ -> typeError pos ("Type " + (ppTyp eTyp) + " is expected to be boolean, (E-Not).") // exF17.java
    | Prim2(ope,e1,e2,_,pos) -> // Rule E-Plus
        let (e1',e1Typ) = typExpr ch ct varEnv e1
        let (e2',e2Typ) = typExpr ch ct varEnv e2
        match (ope,e1Typ,e2Typ) with
          ("+",TypI,TypI) -> (Prim2(ope,e1',e2',Some TypI,pos),TypI)
        | ("-",TypI,TypI) -> (Prim2(ope,e1',e2',Some TypI,pos),TypI)          
        | ("*",TypI,TypI) -> (Prim2(ope,e1',e2',Some TypI,pos),TypI)
        | ("/",TypI,TypI) -> (Prim2(ope,e1',e2',Some TypI,pos),TypI)
        | ("%",TypI,TypI) -> (Prim2(ope,e1',e2',Some TypI,pos),TypI)
        | ("==",_,_) when areEqualityTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)
        | ("!=",_,_) when areEqualityTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)
        | ("<",_,_) when areOrderedTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)
        | (">",_,_) when areOrderedTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)        
        | (">=",TypI,TypI) when areOrderedTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)
        | ("<=",TypI,TypI) when areOrderedTypes(e1Typ,e2Typ) -> (Prim2(ope,e1',e2',Some TypB,pos),TypB)
        | _ -> typeError pos ("Expression " + (ppExpr e1) + " with type " + (ppTyp e1Typ) +
                              " and " + (ppExpr e2) + " with type " + (ppTyp e2Typ) +
                              " do not match expected types for operation " + ope +
                              ", (E-Plus).")  // exF18.java
    | Andalso(e1,e2,_,pos) -> // Rule E-And
        let (e1',e1Typ) = typExpr ch ct varEnv e1
        let (e2',e2Typ) = typExpr ch ct varEnv e2
        match (e1Typ,e2Typ) with
          (TypB,TypB) -> (Andalso(e1',e2',Some TypB,pos),TypB)
        | _ -> typeError pos ("Types " + (ppTyp e1Typ) + " and " + (ppTyp e2Typ) +
                              " are expected to be boolean, (E-And).")  // exF19.java
    | Orelse(e1,e2,_,pos) -> // Rule similar to E-And
        let (e1',e1Typ) = typExpr ch ct varEnv e1
        let (e2',e2Typ) = typExpr ch ct varEnv e2
        match (e1Typ,e2Typ) with
          (TypB,TypB) -> (Orelse(e1',e2',Some TypB,pos),TypB)
        | _ -> typeError pos ("Types " + (ppTyp e1Typ) + " and " + (ppTyp e2Typ) +
                              " are expected to be boolean, (E-Orelse).") // exF20.java
    | PrimC(f,args,_,pos) -> // Rule not covered in book, similar to Prim2. 
        let isSupportedByPrint = function
             TypI | TypB | TypN | TypO _ -> true
           | _            -> false
        let argsTyps = List.map (typExpr ch ct varEnv) args
        let args' = List.map fst argsTyps
        let typs' = List.map snd argsTyps
        match f with
          "print" | "println" ->  // result type is type of last argument - int if no arguments
           let chkTyp (e,t) = chkPremise pos (fun () -> isSupportedByPrint t)  // exF21.java
                                         ("The argument " + (ppExpr e) + " has type " + (ppTyp t) +
                                          " not supported by print, (PrimC).")
           (match argsTyps with
              [] -> (PrimC(f,args',Some TypI,pos),TypI)  // Result type int when no arguments, by design.
            | _ -> List.iter chkTyp argsTyps
                   let lastTyp = List.last typs'
                   (PrimC(f,args',Some lastTyp,pos),lastTyp))  // Result type is type of last argument.
        | _ -> typeError pos ("Primitive " + f + " is not supported, (PrimC).")  // exF08.java
    | New(cn,_,pos) -> // Rule E-New
        chkPremise pos (fun () -> inDom ct cn)  // exF22.java
                   ("The class name " + cn + " has not been declared, (E-New).")
        (New(cn,Some (TypO cn),pos),TypO cn)
    | Call(e,m,_,args,_,pos) -> // Rule E-Invk
        let (e',eTyp) = typExpr ch ct varEnv e
        chkPremise pos (fun () -> isDeclaredClassType ct eTyp)  // exF23.java
                   ("The expression " + (ppExpr e) + ", with type " + (ppTyp eTyp) +
                    ", is not a class type, (E-Invk).")
        let (mthEnv,fldEnv) = lookup ct (getClassName eTyp)  // Will not fail due to check above.
        let argsTyps = List.map (typExpr ch ct varEnv) args
        let args' = List.map fst argsTyps
        let typs' = List.map snd argsTyps 
        let actualSig = (m,typs')

        // Find all more specific method signatures.
        let mthEnv' = List.filter (fun (mthSig,mthTyp) -> isMthMoreSpecific ch actualSig mthSig) mthEnv
        chkPremise pos (fun () -> List.length mthEnv' > 0)  // exF24.java
                   ("A more specific method signature does not exist in class " +
                    (ppTyp eTyp) + " for actual signature " + (ppMthSig actualSig) + ", (E-Invk)")
        let mthEnv'' = List.sortWith (fun (mthSig1,_) (mthSig2,_) ->
                                        if (isMthMoreSpecific ch mthSig1 mthSig2) then -1 else 1) mthEnv'
        let (firstSig,restSigs) = (fst(List.head mthEnv''),List.map fst (List.tail mthEnv''))
        (* CmdLine.debug ("E-Invk" + nl + "  Actual method signature: " + (ppMthSig actualSig) + nl +
                       "  First method signature: " + (ppMthSig firstSig) + nl +
                       "  Rest of method signatures: " + (String.concat ", " (List.map ppMthSig restSigs))) *)
        chkPremise pos (fun () -> List.forall (isMthMoreSpecific ch firstSig) restSigs) // exF25.java
                   ("No unique most specific method signature exists in class " +
                    (ppTyp eTyp) + " for actual signature " + (ppMthSig actualSig) + ", (E-Invk)")
        let (_,(_,rTyp)) = List.head mthEnv''  // We have checked it is non empty above.
        (Call(e',m,Some firstSig,args',Some rTyp,pos),rTyp)

and typAccess (ch:classHierarchy) (ct:ctEnv) (varEnv:varEnv) (a:access) : access*typ =
  let ppExpr = ppExpr false false 0  // Partial apply on ppExpr in Absyn
  match a with
    AccVar(x,_,pos) ->   // Local variable access. Rule AE-This, AE-Super, AE-Var
      chkPremise pos (fun () -> inDom varEnv x)  
                 ("Local variable " + x + " not declared, (AE-Var).")   // exF50.java
      let t = lookup varEnv x
      (AccVar(x,Some t,pos), t)
  | AccFld(e,fld,_,pos) -> // Class field access. Rule AE-Field
      let (e',eTyp) = typExpr ch ct varEnv e
      chkPremise pos (fun () -> isDeclaredClassType ct eTyp) // exF15.java
                 ("The type " + (ppTyp eTyp) + " of " + (ppExpr e) + " is not a class type, (AE-Field).")
      let (mthEnv,fldEnv) = lookup ct (getClassName eTyp)  // Will not fail due to check above.
      chkPremise pos (fun () -> inDom fldEnv fld)
                     ("Field " + fld + " is not declared in class type " + (ppTyp eTyp) + ", (AE-Field).")
      let fldTyp = lookup fldEnv fld  // Fails in exF49.java
      (AccFld(e',fld,Some fldTyp,pos), fldTyp)

// Typing Statements and Declarations

let rec typStmt (ch:classHierarchy) (ct:ctEnv) (mSig:methodSignature) (varEnv:varEnv) (s:stmt) : stmt*varEnv =
  let ppExpr = ppExpr false false 0  // Partial apply on ppExpr in Absyn  
  match s with
  | Expr e -> // Rule S-ExpStmt
    let (e',_) = typExpr ch ct varEnv e
    (Expr e',varEnv)
  | Return(None,pos) -> // Rule S-Return
    let thisTyp = lookup varEnv "this"
    chkPremise pos (fun () -> isDeclaredClassType ct thisTyp)   // Always true, as variable "this" is added to environment by design.
               ("The type " + (ppTyp thisTyp) + " is not a class type, (S-Return).")
    let (mthEnv,fldEnv) = lookup ct (getClassName thisTyp)  // Will not fail due to check above.
    let (_,t:typ) = lookup mthEnv mSig  // Will always succeed as current method signature is added to environment by design.
    chkPremise pos (fun () -> t = TypV) // exF29.java
               ("Non void method with method signature " + (ppMthSig mSig) + ", (S-Return).")
    (Return(None,pos),varEnv)
  | Return(Some e,pos) -> // Rule S-ReturnVal
    let thisTyp = lookup varEnv "this"  // Will always succeed, as variable "this" is added to environmnet by design.
    chkPremise pos (fun () -> isDeclaredClassType ct thisTyp)  // Always true, as variable "this" is added to environment by design.
               ("Type.typStmt.ReturnVal, : the type " + (ppTyp thisTyp) + " is not a class type, (S-ReturnVal).")
    let (mthEnv,fldEnv) = lookup ct (getClassName thisTyp)  // Will not fail due to check above.
    let (_,mthResTyp) = lookup mthEnv mSig // Will always succeed as current method signature is added to environment by design.
    chkPremise pos (fun () -> mthResTyp <> TypV)  // exF31.java
               ("Void method with method signature " + (ppMthSig mSig) + " not allowed, (S-ReturnVal).")
    let (e',eTyp) = typExpr ch ct varEnv e
    chkPremise pos (fun () -> isSubType ch eTyp mthResTyp)  // exF30.java
               ("Expression " + (ppExpr e) + " of type " + (ppTyp eTyp) + " is not a subtype of method result type " +
                (ppTyp mthResTyp) + " for method with signature " + (ppMthSig mSig) + ", (S-ReturnVal).")
    (Return(Some e',pos),varEnv)
  | Block(sds,pos) -> // Rule S-Block
    // Scope of variables within block is only the block.
    let (sdsRev',_) = List.fold (fun (sds, env) sd ->
                                 let (sd',env') = typStmtOrDec ch ct mSig env sd
                                 (sd'::sds,env')) ([],varEnv) sds
    (Block (List.rev sdsRev',pos),varEnv)
  | If(e,s1,s2,pos) -> // Rule S-If
    let (e',eTyp) = typExpr ch ct varEnv e
    chkPremise pos (fun () -> eTyp = TypB)   // exF32.java
               ("Expression " + (ppExpr e) + " of type " + (ppTyp eTyp) + " is not a boolean, (S-If).")
    let (s1',_) = typStmt ch ct mSig varEnv s1
    let (s2',_) = typStmt ch ct mSig varEnv s2
    (If(e',s1',s2',pos),varEnv)
  | While(e,s,pos) ->
    let (e',eTyp) = typExpr ch ct varEnv e
    chkPremise pos (fun () -> eTyp = TypB) // exF33.java
               ("Expression " + (ppExpr e) + " of type " + (ppTyp eTyp) + " is not a boolean, (S-While).")
    let (s',_) = typStmt ch ct mSig varEnv s
    (While(e',s',pos),varEnv)

and typStmtOrDec (ch:classHierarchy) (ct:ctEnv) (mSig:methodSignature) (varEnv:varEnv) (sd:stmtordec) : stmtordec * varEnv =
  match sd with 
    Dec(t,x,pos) -> // Rule D-Var
      chkPremise pos (fun () -> not (inDom varEnv x))  // exF34.java
                 ("Variable " + x + " is already declared, (D-Var).")
      chkPremise pos (fun () -> isBuiltInType t || isClassType ct t) // exF35.java
                 ("Variable " + x + " with type " + (ppTyp t) + " is not a scalar or declared class type, (D-Var).")
      (Dec(t,x,pos), (x,t)::varEnv)
  | Stmt s -> // Rules for statements
    let (s',env) = typStmt ch ct mSig varEnv s
    (Stmt s',varEnv)
  

// Typing Member Declarations

//Rules MD-Field and MD-Method
let typMemberdec (ch:classHierarchy) (ct:ctEnv) (cn:classname) (md:memberdec) : memberdec =
  match md with 
    Methoddec(t,m,pars,s,pos) ->  // Rule MD-Method
      chkPremise pos (fun () -> not(m = "super" || m = "this")) // exF53, exF54
                 ("Method must not be named this or super, (MD-Method).")
      chkPremise pos (fun () -> isMthResType ct t) // exF37.java
                 ("Method " + m + " does not have valid result type " + (ppTyp t) + ", (MD-Method).")
      let pns = List.map snd pars   // Get parameter names
      chkPremise pos (fun () -> List.forall (fun p -> p <> "this" && p <> "super") pns)  // exF03.java, exF04.java
                 ("Method parameters in method " + m + " may not be named this or super, (MD-Method).")
      chkPremise pos (fun () -> allDistinct pns) // exF39.java
                 ("Method parameters must be distinct: " + (String.concat ", " pns) + ", (MD-Method).")
      let chkTyp (t,p) = chkPremise pos (fun () -> isBuiltInType t || isDeclaredClassType ct t) // exF38.java
                                    ("Parameter " + p + " with type " + (ppTyp t) +
                                     " must be a scalar or declared class type, (MD-Method).")
      List.iter chkTyp pars
      let supern = super (findClass ch cn)  // Should never fail - as cn is valid class in ch.
      let varEnv0 = [("this", TypO cn); ("super", TypO supern)]
      let varEnv =  List.fold (fun env (t,p) -> (p,t) :: env) varEnv0 pars
      let mSig = mthSig md
      let (s',_) = typStmt ch ct mSig varEnv s
      Methoddec(t,m,pars,s',pos)
  | Fielddec(t,f,pos) -> // Rule MD-Field
    chkPremise pos (fun () -> f <> "this" && f <> "super") // exF02.java, exF05.java
               ("Field " + f + " may not be named this or super, (MD-Field).")
    chkPremise pos (fun () -> isBuiltInType t || isDeclaredClassType ct t) // exF36.java
               ("Field " + f + " of type " + (ppTyp t) + " must be scalar or declared class type, (MD-Field).")
    Fielddec(t,f,pos)
  
// Type Program

// Rule: Class
let typClassdec (ch:classHierarchy) (ct:ctEnv) (cd:classdec) : classdec =
  match cd with
    Classdec(cn,supern,mds,pos) -> let mds' = List.map (typMemberdec ch ct cn) mds
                                   Classdec(cn,supern,mds',pos)

// Rule: Prog
let typProg (ch:classHierarchy) : classHierarchy =
  let ct = buildCT ch  // Build class table
  CmdLine.verbose (sprintf "%s" (ppCtEnv ct))
  let rec loop (Hierarchy(cd, chs)) =   // Type classes depth first order.
    let cd' = typClassdec ch ct cd
    let chs' = List.map loop chs
    Hierarchy(cd', chs')
  let ch' = loop ch

  // Check for exactly one main method in class hierarchy with type annotations.
  match tryFindClass ch' "Main" with
      None -> typeError emptyPos ("Type error: " + nl + "  Mandatory class with name Main does not exist, (Prog).")  // exF40.java
    | Some (Classdec(_,_,_,pos)) ->
        let (mthEnv,fldEnv) = lookup ct "Main"  // Will not fail as "Main" class does exist.
        let mainMths = List.filter (fun ((mthName,_),_) -> mthName = "main") mthEnv
        let numMainMths = List.length mainMths
        chkPremise pos (fun () -> numMainMths = 1) // exF41.java, exF46.java
                   (if numMainMths > 1
                      then ("Class Main has " + (string)numMainMths +
                            " methods named main. Must have exactly one, (Prog).")
                      else ("Class Main has no methods named main. Must have exactly one, (Prog)."))
        let (_,(typs,rTyp)) = List.head mainMths // We have checked it is non empty above.  
        // Could use position of main method instead of class - would
        // require access to Methoddec, which may recide in other
        // class.
        chkPremise pos (fun () -> rTyp = TypV)  // exF42.java
                   ("Result type of main method in class Main is not void, but " + (ppTyp rTyp) + ", (Prog).")
        chkPremise pos (fun () -> List.forall isScalarType typs)  // exF43.java
                   ("Parameter types of main method in class Main are not all scalar types, (Prog).")
  ch'
