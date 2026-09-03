(* Programming language concepts, 2026-02-20 *)

(* Evaluation, checking, and compilation of object language expressions *)
(* Stack machines for expression evaluation                             *) 

(* Object language expressions with variable bindings and nested scope *)

module Intcomp1

type expr = 
  | CstI of int
  | Var of string
  | Let of (string * expr) list * expr (* (From assignment 1) | CHANGED 1.2.1 *)
  | Prim of string * expr * expr;;

(* Some closed expressions: *)

let e0 = Prim("+", CstI 17, Prim("+", CstI 5, CstI 7));;
let e1 = Let([("z", CstI 17)], Prim("+", Var "z", Var "z"));;

let e2 = Let([("z", CstI 17)], 
             Prim("+", Let([("z", CstI 22)], Prim("*", CstI 100, Var "z")),
                       Var "z"));;

let e3 = Let([("z", Prim("-", CstI 5, CstI 4))], 
             Prim("*", CstI 100, Var "z"));;

let e4 = Prim("+", Prim("+", CstI 20, Let([("z", CstI 17)], 
                                          Prim("+", Var "z", CstI 2))),
                   CstI 30);;

let e5 = Prim("*", CstI 2, Let([("x", CstI 3)], Prim("+", Var "x", CstI 4)));;

let e6 = Let([("z", Var "x")], Prim("+", Var "z", Var "x"))
let e7 = Let([("z", CstI 3)], Let([("y", Prim("+", Var "z", CstI 1))], Prim("+", Var "z", Var "y")))

let e8 = Let([("z", Let([("x", CstI 4)], Prim("+", Var "x", CstI 5)))], Prim("*", Var "z", CstI 2))

let e9 = Let([("z", CstI 3)], Let([("y", Prim("+", Var "z", CstI 1))], Prim("+", Var "x", Var "y")))

let e10 = Let([("z", Prim("+", Let([("x", CstI 4)], Prim("+", Var "x", CstI 5)), Var "x"))], Prim("*", Var "z", CstI 2))


(* all are variables in the example below are bound *)
let list_let_test_all_bound = Let(["a", CstI 2; "b", e1; "c", e5], Prim("+", Var "a", Prim("+", Var "b", Var "c")))
(* e6 features unbound var "x" and "d" is also unbound *)
let list_let_test_some_free = Let(["a", CstI 2; "b", e1; "c", e6 ], Prim("+", Prim("+", Var "d", Var "a"), Prim("+", Var "b", Var "c")))

(* e6 features unbound var "x" and "d" is also unbound *)
let list_x1_in_x2 = Let(["x1", CstI 1; "x2", Prim("+", Var "x1", CstI 1)], Prim("+", Prim("+", Var "x1", Var "x2"), Prim("+", Var "x2", Var "x1")))
(* ---------------------------------------------------------------------- *)

(* Evaluation of expressions with variables and bindings *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* (From assignment 1) CHANGED | 2.1 *)
let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Let(erhs, ebody) -> 
      (* for each let binding with variable "x"
        - evaluate the let binding with the accumulated environment thus far
        - add the evaluated variable to the accumulated environment

        The idea behind the accumulated env, is that this type of let binding should be supported:
            "let a = 1, b = a + a in a + b"
      *)
      let env1 = List.fold (fun envAcc (x, erhs) ->  (x, eval erhs envAcc) :: envAcc) env erhs
      eval ebody env1
    | Prim("+", e1, e2) -> eval e1 env + eval e2 env (* Case for each supported binary operation *)
    | Prim("*", e1, e2) -> eval e1 env * eval e2 env
    | Prim("-", e1, e2) -> eval e1 env - eval e2 env
    | Prim _            -> failwith "unknown primitive";;

let run e = eval e [];;
let res = List.map run [e1;e2;e3;e4;e5;e7]  (* e6 has free variables *)


(* ---------------------------------------------------------------------- *)

(* Closedness *)

// let mem x vs = List.exists (fun y -> x=y) vs;;

let rec mem x vs = 
    match vs with
    | []      -> false
    | v :: vr -> x=v || mem x vr;;

(* Checking whether an expression is closed.  The vs is 
   a list of the bound variables.  *)

(* (From assignment 1) CHANGED *)
let rec closedin (e : expr) (vs : string list) : bool =
    match e with
    | CstI i -> true
    | Var x  -> List.exists (fun y -> x=y) vs
    | Let(erhs, ebody) -> 
       (* 
        for each let binding with variable "x"
        - Recursively check that the body of the let binding with the current accumulated environment is closed
        - Add "x" to the accumulated environment
       *)
      let vs1, isClosedInLetBinding = List.fold (fun (vsAcc, result) (x, xbody) -> (x :: vsAcc, result && (closedin xbody vsAcc))) (vs, true) erhs
      isClosedInLetBinding && closedin ebody vs1
//      let vs1 = List.fold (fun acc (x, _) -> x :: acc) vs erhs
      // List.forall (fun (_, body) -> closedin body vs) erhs && closedin ebody vs1
    | Prim(ope, e1, e2) -> closedin e1 vs && closedin e2 vs;;

(* An expression is closed if it is closed in the empty environment *)

let closed1 e = closedin e [];;
let _ = List.map closed1 [e1;e2;e3;e4;e5;e6;e7;e8;e9;e10]

(* ---------------------------------------------------------------------- *)

(* Substitution of expressions for variables *)

(* This version of lookup returns a Var(x) expression if there is no
   pair (x,e) in the list env --- instead of failing with exception: *)

let rec lookOrSelf env x =
    match env with 
    | []        -> Var x
    | (y, e)::r -> if x=y then e else lookOrSelf r x;;

(* Remove (x, _) from env: *)

let rec remove env x =
    match env with 
    | []        -> []
    | (y, e)::r -> if x=y then r else (y, e) :: remove r x;;

(* Naive substitution, may capture free variables: *)

(* (From assignment 1) CHANGED *)
let rec nsubst (e : expr) (env : (string * expr) list) : expr =
    match e with
    | CstI i -> e
    | Var x  -> lookOrSelf env x
    | Let(erhs, ebody) ->
      (* Remove every variable defined in the let binding from env *)
      let newenv = List.fold (fun acc (x, _) -> remove acc x) env erhs

      (* Perform each subst in the body of each expression in the let binding *)
      let subst_erhs = List.map (fun (x, ebody) -> (x, nsubst ebody env)) erhs

      Let(subst_erhs, nsubst ebody newenv)
    | Prim(ope, e1, e2) -> Prim(ope, nsubst e1 env, nsubst e2 env)

(* Some expressions with free variables: *)

let e6s0 = Prim("+", Var "y", Var "z");;

let e6s1 = nsubst e6s0 [("z", CstI 17)];;

let e6s2 = nsubst e6s0 [("z", Prim("-", CstI 5, CstI 4))];;

let e6s3 = nsubst e6s0 [("z", Prim("+", Var "z", Var "z"))];;

// Shows that only z outside the Let gets substituted:
let e7s0 = Prim("+", Let([("z", CstI 22)], Prim("*", CstI 5, Var "z")),
                   Var "z");;

let e7s1 = nsubst e7s0 [("z", CstI 100)];;

// Shows that only the z in the Let rhs gets substituted
let e8s0 = Let([("z", Prim("*", CstI 22, Var "z"))], Prim("*", CstI 5, Var "z"));;

let e8s1 = nsubst e8s0 [("z", CstI 100)];;

// Shows (wrong) capture of free variable z under the let:
let e9s0 = Let([("z", CstI 22)], Prim("*", Var "y", Var "z"));;

let e9s1 = nsubst e9s0 [("y", Var "z")];;

// 
let e9s2 = nsubst e9s0 [("z", Prim("-", CstI 5, CstI 4))];;

let newVar : string -> string = 
    let n = ref 0
    let varMaker s = (n.Value <- n.Value + 1; s + string (n.Value))
    varMaker

(* Correct, capture-avoiding substitution *)

(* (From assignment1) CHANGED *)
let rec subst (e : expr) (env : (string * expr) list) : expr =
    match e with
    | CstI i -> e
    | Var x  -> lookOrSelf env x
    | Let(erhs, ebody) ->
      (* For each variable defined int the let binding: Generate a unique name *)
      let renamed_xs = List.map (fun (x, ebody) -> (x, newVar x, ebody)) erhs

      (* Replace every old x with x' where x' is the unique name generated for x earlier *)
      let newenv = List.fold (fun acc (x, x', ebody) -> (x, Var x') :: remove acc x) env renamed_xs

      (* Recursively perform subst on each let binding expression *)
      let subst_erhs = List.map (fun (_, x', ebody) -> (x', subst ebody env)) renamed_xs
      Let(subst_erhs, subst ebody newenv)
    | Prim(ope, e1, e2) -> Prim(ope, subst e1 env, subst e2 env) (* Recursively perform subst in each branch of the tree *)

let e6s1a = subst e6 [("z", CstI 17)];;

let e6s2a = subst e6 [("z", Prim("-", CstI 5, CstI 4))];;

let e6s3a = subst e6 [("z", Prim("+", Var "z", Var "z"))];;


// Shows renaming of bound variable z (to z1)
let e7s1a = subst e7s0 [("z", CstI 100)];;

// Shows renaming of bound variable z (to z2)
let e8s1a = subst e8s0 [("z", CstI 100)];;

// Shows renaming of bound variable z (to z3), avoiding capture of free z
let e9s1a = subst e9s0 [("y", Var "z")];;

(* ---------------------------------------------------------------------- *)

(* Free variables *)

(* Operations on sets, represented as lists.  Simple but inefficient;
   one could use binary trees, hashtables or splaytrees for
   efficiency.  *)

(* union(xs, ys) is the set of all elements in xs or ys, without duplicates *)

let rec union (xs, ys) = 
    match xs with 
    | []    -> ys
    | x::xr -> if mem x ys then union(xr, ys)
               else x :: union(xr, ys);;

(* minus xs ys  is the set of all elements in xs but not in ys *)

let rec minus (xs, ys) = 
    match xs with 
    | []    -> []
    | x::xr -> if mem x ys then minus(xr, ys)
               else x :: minus (xr, ys);;

(* Find all variables that occur free in expression e *)
(* (From assignment 1) CHANGED *)
let rec freevars e : string list =
    match e with
    | CstI _ -> [] (* Constant is not variable *)
    | Var x -> [ x ] (* Assume it's free for now, we'll get rid of it later if it's not *)
    | Let(exps, body) ->

        (* For each expression in a let bind with variable "X":
            - recursively find free vars
            - subtraact occured vars from the aforementioned free vars
            - add "x" to the list of occurred vars, and add all of the free vars to the free vars list
        *)
        let occuredVariables, free =
            List.fold
                (fun (occuredVariables, free) (x, erhs) ->
                    let f' = minus (freevars erhs, occuredVariables) in (x :: occuredVariables, f' @ free))
                ([], [])
                exps
        (* union free vars foudn in the let expression, with the free vars foud in the body of the let expression *)
        union (free, minus (freevars body, occuredVariables))
    | Prim(_, e1, e2) -> union (freevars e1, freevars e2) (* union the free vars of each expression in the binary operation *)

(* Alternative definition of closed *)

let closed2 e = (freevars e = []);;
let _ = List.map closed2 [e1;e2;e3;e4;e5;e6;e7;e8;e9;e10]

(* ---------------------------------------------------------------------- *)

(* Compilation to target expressions with numerical indexes instead of
   symbolic variable names.  *)

type texpr =                            (* target expressions *)
  | TCstI of int
  | TVar of int                         (* index into runtime environment *)
  | TLet of texpr * texpr               (* erhs and ebody                 *)
  | TPrim of string * texpr * texpr;;


(* Map variable name to variable index at compile-time *)

let rec getindex vs x = 
    match vs with 
    | []    -> failwith "Variable not found"
    | y::yr -> if x=y then 0 else 1 + getindex yr x;;

(* Compiling from expr to texpr *)

(* (From assignment 1) CHANGED *)
let rec tcomp (e : expr) (cenv : string list) : texpr =
    match e with
    | CstI i -> TCstI i
    | Var x  -> TVar (getindex cenv x)
    | Let(erhs, ebody) -> 
        (* 
            recurse through the list of let expressions
            - create a texpr for each element in the let expression,
            - add the variable name of the let expression to the accumulated environment 
        *)
        let rec aux (erhs' : (string * expr) list) (cenv' : string list) : texpr =
            match erhs' with
            | [] -> tcomp ebody cenv
            | (x, body) :: xs -> let cenv'' = x :: cenv' in TLet(tcomp body cenv', aux xs cenv'')
        aux erhs cenv
//      let cenv1 = x :: cenv 
//      TLet(tcomp erhs cenv, tcomp ebody cenv1)
    | Prim(ope, e1, e2) -> TPrim(ope, tcomp e1 cenv, tcomp e2 cenv);;

(* Evaluation of target expressions with variable indexes.  The
   run-time environment renv is a list of variable values (ints).  *)

let rec teval (e : texpr) (renv : int list) : int =
    match e with
    | TCstI i -> i
    | TVar n  -> List.item n renv
    | TLet(erhs, ebody) -> 
      let xval = teval erhs renv
      let renv1 = xval :: renv 
      teval ebody renv1 
    | TPrim("+", e1, e2) -> teval e1 renv + teval e2 renv
    | TPrim("*", e1, e2) -> teval e1 renv * teval e2 renv
    | TPrim("-", e1, e2) -> teval e1 renv - teval e2 renv
    | TPrim _            -> failwith "unknown primitive";;

(* Correctness: eval e []  equals  teval (tcomp e []) [] *)


(* ---------------------------------------------------------------------- *)

(* Stack machines *)

(* Stack machine instructions.  An expressions in postfix or reverse
   Polish form is a list of stack machine instructions. *)

type rinstr =
  | RCstI of int
  | RAdd 
  | RSub
  | RMul 
  | RDup
  | RSwap;;

(* A simple stack machine for evaluation of variable-free expressions
   in postfix form *)

let rec reval (inss : rinstr list) (stack : int list) : int =
    match (inss, stack) with 
    | ([], v :: _) -> v
    | ([], [])     -> failwith "reval: no result on stack!"
    | (RCstI i :: insr,             stk)  -> reval insr (i::stk)
    | (RAdd    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1+i2)::stkr)
    | (RSub    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1-i2)::stkr)
    | (RMul    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1*i2)::stkr)
    | (RDup    :: insr,       i1 :: stkr) -> reval insr (i1 :: i1 :: stkr)
    | (RSwap   :: insr, i2 :: i1 :: stkr) -> reval insr (i1 :: i2 :: stkr)
    | _ -> failwith "reval: too few operands on stack";;

let rpn1 = reval [RCstI 10; RCstI 17; RDup; RMul; RAdd] [];;

(* Compilation of a variable-free expression to a rinstr list *)

let rec rcomp (e : expr) : rinstr list =
    match e with
    | CstI i            -> [RCstI i]
    | Var _             -> failwith "rcomp cannot compile Var"
    | Let _             -> failwith "rcomp cannot compile Let"
    | Prim("+", e1, e2) -> rcomp e1 @ rcomp e2 @ [RAdd]
    | Prim("*", e1, e2) -> rcomp e1 @ rcomp e2 @ [RMul]
    | Prim("-", e1, e2) -> rcomp e1 @ rcomp e2 @ [RSub]
    | Prim _            -> failwith "unknown primitive";;
            
(* Correctness: eval e []  equals  reval (rcomp e) [] *)
eval e0 [];;
rcomp e0;;
reval (rcomp e0) [];;



(* Storing intermediate results and variable bindings in the same stack *)

type sinstr =
  | SCstI of int                        (* push integer           *)
  | SVar of int                         (* push variable from env *)
  | SAdd                                (* pop args, push sum     *)
  | SSub                                (* pop args, push diff.   *)
  | SMul                                (* pop args, push product *)
  | SPop                                (* pop value/unbind var   *)
  | SSwap;;                             (* exchange top and next  *)
 
let rec seval (inss : sinstr list) (stack : int list) =
    match (inss, stack) with
    | ([], v :: _) -> v
    | ([], [])     -> failwith "seval: no result on stack"
    | (SCstI i :: insr,          stk) -> seval insr (i :: stk) 
    | (SVar i  :: insr,          stk) -> seval insr (List.item i stk :: stk) 
    | (SAdd    :: insr, i2::i1::stkr) -> seval insr (i1+i2 :: stkr)
    | (SSub    :: insr, i2::i1::stkr) -> seval insr (i1-i2 :: stkr)
    | (SMul    :: insr, i2::i1::stkr) -> seval insr (i1*i2 :: stkr)
    | (SPop    :: insr,    _ :: stkr) -> seval insr stkr
    | (SSwap   :: insr, i2::i1::stkr) -> seval insr (i1::i2::stkr)
    | _ -> failwith "seval: too few operands on stack";;


(* A compile-time variable environment representing the state of
   the run-time stack. *)

type stackvalue =
  | Value                               (* A computed value *)
  | Bound of string;;                   (* A bound variable *)

(* Compilation to a list of instructions for a unified-stack machine *)

(* (From assignment 1) CHANGED *)
let rec scomp (e : expr) (cenv : stackvalue list) : sinstr list =
    match e with
    | CstI i -> [SCstI i]
    | Var x  -> [SVar (getindex cenv (Bound x))]
    | Let(erhs, ebody) -> 
          (* Not a required exercise, so we just made this so that the program would compile *)
          (* recursively compiles each expression and adds the variable to the accumulated environment.
             concats the compiled expressions onto the compile body
          *)
          let erhs_scomped, cenv' = List.fold (fun (scomp_acc, cenv_acc) (x, xbody) -> scomp xbody cenv_acc @ scomp_acc, Bound x :: cenv_acc) ([], cenv) erhs
          erhs_scomped @ scomp ebody cenv' @ [SSwap; SPop]
    | Prim("+", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SAdd] 
    | Prim("-", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SSub] 
    | Prim("*", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SMul] 
    | Prim _ -> failwith "scomp: unknown operator";;

let s1 = scomp e1 [];;
let s2 = scomp e2 [];;
let s3 = scomp e3 [];;
let s5 = scomp e5 [];;

(* Output the integers in list inss to the text file called fname: *)

let intsToFile (inss : int list) (fname : string) = 
    let text = String.concat " " (List.map string inss)
    System.IO.File.WriteAllText(fname, text);;

(* CHANGED (2.4) *)
(*
  Map each instruction to the corresponding stack machine bytecode.
  Uses a continuation to make the function tail-recursive
 *)
let assemble (ins : sinstr list) : int list =
    let rec aux (ins : sinstr list) (cont : int list -> int list) =
        match ins with
        | []      -> cont []
        | x :: xs -> match x with
                     | SCstI i  -> aux xs (fun ys -> (cont (0 :: i :: ys)))
                     | SVar  i  -> aux xs (fun ys -> (cont (1 :: i :: ys)))
                     | SAdd     -> aux xs (fun ys -> (cont (2 :: ys)))
                     | SSub     -> aux xs (fun ys -> (cont (3 :: ys)))
                     | SMul     -> aux xs (fun ys -> (cont (4 :: ys)))
                     | SPop     -> aux xs (fun ys -> (cont (5 :: ys)))
                     | SSwap    -> aux xs (fun ys -> (cont (6 :: ys)))
    aux ins id

(* -----------------------------------------------------------------  *)
