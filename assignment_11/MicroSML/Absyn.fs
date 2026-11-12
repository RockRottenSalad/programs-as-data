// Fun/Absyn.fs * Abstract syntax for micro-SML, a functional language.

module Absyn

type expr<'a,'b> = 
  | CstI of int * 'a option
  | CstB of bool * 'a option
  | CstN of 'a option
  | Var of string * 'a option
  | Pair of expr<'a,'b> * expr<'a,'b> * 'a option // CHANGED | 13.2
  | AndAlso of expr<'a,'b> * expr<'a,'b> * 'a option
  | OrElse of expr<'a,'b> * expr<'a,'b> * 'a option
  | Seq of expr<'a,'b> * expr<'a,'b> * 'a option
  | Prim2 of string * expr<'a,'b> * expr<'a,'b> * 'a option
  | Prim1 of string * expr<'a,'b> * 'a option
  | If of expr<'a,'b> * expr<'a,'b> * expr<'a,'b>
  | Fun of string * expr<'a,'b> * 'a option 
  | Call of expr<'a,'b> * expr<'a,'b> * bool option * 'a option
  | Let of valdec<'a,'b> list * expr<'a,'b>
  | Raise of expr<'a,'b> * 'a option
  | TryWith of expr<'a,'b> * exnvar * expr<'a,'b>
and valdec<'a,'b> =
  | Fundecs of (string * string * expr<'a,'b> * 'b option) list  // Top level mutual recursive function declarations
  | Valdec of string * expr<'a,'b> * 'b option
  | Exndec of exnvar
and exnvar =
  | ExnVar of string    
and program<'a,'b> =
  | Prog of valdec<'a,'b> list * expr<'a,'b> // A program is a list of top level declarations and an expression

let ppExnVar = function
  | ExnVar exn -> exn      

let ppTail = function
    None -> " "
  | Some true -> "_tail "
  | Some false -> " "
  
let indent i = Util.indent i ""

let rec ppExpr' fPPa fPPb i e =
  match e with
  | CstI (i,aOpt) -> sprintf "%d" i + (fPPa aOpt)
  | CstB (b,aOpt) -> sprintf "%b" b + (fPPa aOpt)
  | CstN aOpt     -> sprintf "nil" + (fPPa aOpt)
  | Var (x,aOpt)  -> sprintf "%s" x + (fPPa aOpt)
  | Pair (l,r,aOpt)  -> "(" + (ppExpr' fPPa fPPb i l) + "," + (ppExpr' fPPa fPPb i r) + ")" + (fPPa aOpt)
  | AndAlso(e1,e2,aOpt) -> "(" + (ppExpr' fPPa fPPb i e1) + " && " + (ppExpr' fPPa fPPb i e2) + ")" + (fPPa aOpt)
  | OrElse (e1,e2,aOpt) -> "(" + (ppExpr' fPPa fPPb i e1) + " || " + (ppExpr' fPPa fPPb i e2) + ")" + (fPPa aOpt)
  | Seq(e1,e2,aOpt) -> "(" + (ppExpr' fPPa fPPb i e1) + " ; " + (ppExpr' fPPa fPPb i e2) + ")" + (fPPa aOpt)
  | Let(valdecs,letBody) ->
      "\n" + (indent (i+2)) + "let\n" + (ppValdecs fPPa fPPb (i+4) valdecs) + "\n" + (indent (i+2)) + "in\n" +
      (indent (i+4)) + (ppExpr' fPPa fPPb (i+2) letBody) + "\n" + (indent (i+2)) + "end"
  | Prim2(ope,e1,e2,aOpt) -> "(" + (ppExpr' fPPa fPPb i e1) + " " + ope + " " + (ppExpr' fPPa fPPb i e2) + ")" +
                             (fPPa aOpt)
  | Prim1(ope,e,aOpt) -> ope + "(" + (ppExpr' fPPa fPPb i e) + ")" + (fPPa aOpt)
  | If(e1,e2,e3) -> "if " + (ppExpr' fPPa fPPb i e1) + " then " + (ppExpr' fPPa fPPb i e2) + " else " + (ppExpr' fPPa fPPb i e3)
  | Fun(x,e,aOpt) -> "fn " + x + " -> " + (ppExpr' fPPa fPPb i e) + (fPPa aOpt)
  | Call(e1,e2,tOpt,aOpt) -> (ppExpr' fPPa fPPb i e1) + (ppTail tOpt) + (ppExpr' fPPa fPPb i e2) + (fPPa aOpt)
  | Raise(e,aOpt) -> "raise " + (ppExpr' fPPa fPPb i e) + (fPPa aOpt)
  | TryWith(e1,exn,e2) -> "\n" + (indent (i+2)) + "(try " + (ppExpr' fPPa fPPb (i+4) e1) +
                          "\n" + (indent (i+2)) + "with " + (ppExnVar exn) + " -> " + (ppExpr' fPPa fPPb (i+4) e2) + ")"
and ppValDec' fPPa fPPb i = function
  | Fundecs fs -> ppFundec fPPa fPPb i fs
  | Valdec(x,eRhs,bOpt) -> ppValdec fPPa fPPb i (x,eRhs,bOpt)
  | Exndec(ExnVar x) -> (indent i) + "exception " + x 
and ppValdecs fPPa fPPb i valdecs = String.concat "\n" (List.map (ppValDec' fPPa fPPb i) valdecs)
and ppFundec fPPa fPPb i fs = 
  let fsPP = List.map (fun (f,x,body,bOpt) -> f  + (fPPb bOpt) + " " + x + " = " + (ppExpr' fPPa fPPb i body)) fs
  (indent i) + "fun " + (String.concat ("\n" + (indent i) + "and ") fsPP)
and ppValdec fPPa fPPb i (x,eRhs,bOpt) =
  (indent i) + "val " + x + (fPPb bOpt) + " = " + (ppExpr' fPPa fPPb i eRhs)

let ppExpr fPPa fPPb e : string = ppExpr' fPPa fPPb 0 e

let ppProg fPPa fPPb p : string =
  let ppProg' i = function
    | Prog(valdecs,e) ->
      ppValdecs fPPa fPPb i valdecs + "\n" + (indent i) + "begin\n" +
        (indent 2) + (ppExpr' fPPa fPPb (i+2) e) + "\n" + (indent i) + "end"
  ppProg' 0 p

let rec getOptExpr e : 'a Option =
  match e with
  | CstI (i,aOpt) -> aOpt
  | CstB (b,aOpt) -> aOpt
  | CstN aOpt -> aOpt
  | Var (x,aOpt)  -> aOpt
  | Pair(_, _, aOpt) -> aOpt
  | AndAlso(_,_,aOpt) -> aOpt
  | OrElse(_,_,aOpt) -> aOpt
  | Seq(_,_,aOpt) -> aOpt
  | Prim2(ope,e1,e2,aOpt) -> aOpt
  | Prim1(ope,e,aOpt) -> aOpt
  | If(e1,e2,e3) -> getOptExpr e3       // e2 and e3 has same type
  | Fun(x,e,aOpt) -> aOpt
  | Call(e1,e2,t,aOpt) -> aOpt
  | Raise(e,aOpt) -> aOpt
  | TryWith(e1,exn,e2) -> getOptExpr e1 // e1 and e3 has same type
  | Let(_,letBody) -> getOptExpr letBody

let tailcalls p : program<'a,'b> =
  let rec tc' tPos e =
    match e with
    | CstI _ -> e
    | CstB _ -> e
    | CstN _ -> e
    | Var _ -> e
    | Pair(l, r, aOpt) -> Pair(tc' false l, tc' true r, aOpt)
    | AndAlso(e1,e2,aOpt) -> AndAlso(tc' false e1,tc' tPos e2,aOpt)
    | OrElse(e1,e2,aOpt) -> OrElse(tc' false e1,tc' tPos e2,aOpt)
    | Seq(e1,e2,aOpt) -> Seq(tc' false e1,tc' tPos e2,aOpt)
    | Prim2(ope,e1,e2,aOpt) -> Prim2(ope,tc' false e1,tc' false e2,aOpt)
    | Prim1(ope,e,aOpt) -> Prim1(ope,tc' false e,aOpt)
    | If(e1,e2,e3) -> If(tc' false e1,tc' tPos e2,tc' tPos e3)
    | Fun(x,e,aOpt) -> Fun(x,tc' true e,aOpt)
    | Call(e1,e2,_,aOpt) -> Call(tc' false e1,tc' false e2,Some tPos,aOpt)
    | Let(valdecs,letBody) -> Let(List.map (tcValdec' false) valdecs,tc' tPos letBody)
    | Raise(e1,aOpt) -> e
      // An exception handler must be popped after e1
    | TryWith(e1,exn,e2) -> TryWith(tc' false e1, exn, tc' tPos e2) 
  and tcValdec' tPos = function
    | Valdec(x,eRhs,bOpt) -> Valdec(x,tc' tPos eRhs,bOpt)
    | Fundecs fs -> Fundecs(List.map (fun (f,x,e,bOpt) -> (f,x,tc' true e,bOpt)) fs)
    | Exndec(x) -> Exndec(x)
  and tcProg' = function
    | Prog (valdecs,body) -> Prog(List.map (tcValdec' false) valdecs,tc' true body)
  tcProg' p

let ppFreevars fvs =
  "Freevars = [ " + (String.concat "," fvs) + " ]\n"

// Global variable used for the exception number generator.
let exnNumVar = "__exnNum__"  

let rec freevars e : string Set =
  match e with 
  | CstI (i,_) -> Set.empty
  | CstB (b,_) -> Set.empty
  | CstN _     -> Set.empty
  | Pair (l, r, _) -> freevars l + freevars r (* CHANGED | 13.2 *)
  | Var (x,_)  -> set [x]
  | Prim1(ope,e1,_) -> freevars e1
  | Prim2(ope,e1,e2,_) -> (freevars e1) + (freevars e2)
  | AndAlso(e1,e2,_) -> (freevars e1) + (freevars e2)
  | OrElse(e1,e2,_) -> (freevars e1) + (freevars e2)
  | Seq(e1,e2,_) -> (freevars e1) + (freevars e2)
  | Let(valdecs,letBody) ->
    // Below (... +fvs - bvs) assumes alpha conversion. See ex11.sml for an example where
    //   it fails. Alpha conversion is covered as an exercise.
    let (fvs,bvs) = List.fold freevarsValdec (Set.empty, Set.empty) valdecs
    (freevars letBody) + fvs - bvs 
  | If(e1, e2, e3) -> (freevars e1) + (freevars e2) + (freevars e3)
  | Fun(x,fBody,_) -> freevars fBody - (set [x])
  | Call(eFun, eArg,_,_) -> freevars eFun + (freevars eArg)
  | Raise(e1,_) -> freevars e1
  | TryWith(e1,ExnVar exn,e2) -> (freevars e1) + (set [exn]) + (freevars e2) // exn is also free
and freevarsValdec (fvs, bvs) = function // bvs are bound variables, either globally or locally.
    Valdec(x,eRhs,_) -> (fvs + ((freevars eRhs) - set [x]),bvs + set [x])
  | Exndec (ExnVar exn) -> (fvs + set [exnNumVar],bvs + set [exn]) 
  | Fundecs fs -> 
    let fEnv = Set.ofList (List.map (fun (f,_,_,_) -> f) fs) // fBody may recursively call f
    let funFree =
      List.foldBack (fun (f,x,fBody,_) acc ->
                       (acc + (freevars fBody - fEnv - set [x]))) fs fvs
    (funFree, bvs + fEnv)
    
// Alpha conversion is implemented as an exercise.
// Exampels ex11.sml, ex15.sml and ex16.sml do not work without alpha
// conversion.
let alphaConv p : program<'a,'b> = p




