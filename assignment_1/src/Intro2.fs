(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [("a", 3); ("c", 78); ("baf", 666); ("b", 111)];;

let emptyenv = []; (* the empty environment *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

let cvalue = lookup env "c";;


(* Object language expressions with variables *)

type expr = 
  | CstI of int
  | Var of string
  | Prim of string * expr * expr
  | If of expr * expr * expr

(* 1.7 | 1.2 *)

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;

(* 1.7 | 1.2 *)
let e4 = Prim("min", Prim("*", Var "a", CstI 2), Var "a");;
let e5 = Prim("max", Prim("*", Var "a", CstI 2), Var "a");;

let e6 = Prim("==", Prim("*", Var "a", CstI 2), Var "a");;
let e7 = Prim("==", Var "a", Var "a");;

(* Evaluation within an environment *)

(* 1.7 | 1.3 + 1.4 + 1.5*)
let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | If(guard, e1, e2) -> if eval guard env <> 0 then eval e1 env else eval e2 env
    | Prim(ope, e1, e2) ->
        let i1 = eval e1 env
        let i2 = eval e2 env
        match ope with
        | "+"   -> i1 + i2
        | "*"   -> i1 * i2
        | "-"   -> i1 - i2
        | "max" -> max i1 i2
        | "min" -> min i1 i2
        | "=="  -> if i1 = i2 then 1 else 0
        | _     -> failwith "unknown primitive"

let e1v  = eval e1 env;;
let e2v1 = eval e2 env;;
let e2v2 = eval e2 [("a", 314)];;
let e3v  = eval e3 env;;

let e4v  = eval e4 env;;
let e5v  = eval e5 env;;
let e6v  = eval e6 env;;
let e7v  = eval e7 env;;


