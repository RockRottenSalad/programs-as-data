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

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;


(* Evaluation within an environment *)

let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i              -> i
    | Var x               -> lookup env x
    | If (e1, e2, e3)     -> if not (eval e1 env = 0) then eval e2 env else eval e3 env
    | Prim(ope, e1, e2)   ->
        let i1 = eval e1 env
        let i2 = eval e2 env
        match ope with
        | "+" -> i1 + i2
        | "*" -> i1 * i2
        | "-" -> i1 - i2
        | "==" -> if i1 = i2 then 1 else 0
        | "Max" -> max i1 i2
        | "Min" -> min i1 i2
        | _ -> failwith "unknown primitive"

let e1v  = eval e1 env;;
let e2v1 = eval e2 env;;
let e2v2 = eval e2 [("a", 314)];;
let e3v  = eval e3 env;;

let isEqual1 = eval (Prim("==", e1, e2)) env;; // should return 1
let isEqual2 = eval (Prim("==", e1, e2)) [("a", 15)];; // should return 0
let findMax = eval (Prim("Max", e1, e2)) [("a", 15)];; // should return 15
let findMin = eval (Prim("Min", e1, e2)) [("a", 15)];; // should return 3

type aexpr = 
  | CstI of int
  | Var of string
  | Add of aexpr * aexpr
  | Mul of aexpr * aexpr
  | Sub of aexpr * aexpr

let rep1 = Sub(Var "v" ,Mul(Var "w", Var "z"))
let rep2 = Mul(CstI 2, Sub(Var "v", Add(Var "w", Var "z")))
let rep3 = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")))

let rec fmt (e : aexpr) :string =
  match e with
  | CstI i  -> string i
  | Var c -> c
  | Add (e1, e2) -> string "(" + fmt e1 + " + " + fmt e2 + ")"
  | Sub (e1, e2) -> string "(" + fmt e1 + " - " + fmt e2 + ")"
  | Mul (e1, e2) -> string "(" + fmt e1 + " * " + fmt e2 + ")"

let rec simplify (e : aexpr) : aexpr =
  match e with
  | CstI i  -> CstI i
  | Var c -> Var c
  | Add (e1, e2) -> 
    match simplify e1, simplify e2 with
    | x, CstI 0 -> x
    | CstI 0, y -> y
    | x, y -> Add (x,y)
  | Sub (e1, e2) ->
    match simplify e1, simplify e2 with
    | x, CstI 0 -> x
    | CstI 0, y -> y
    | x, y when x = y -> CstI 0
    | x, y -> Sub (simplify x, simplify y)
  | Mul (e1, e2) ->
    match simplify e1, simplify e2 with
    | _ , CstI 0 -> CstI 0
    | CstI 0, _ -> CstI 0
    | x, CstI 1 -> x
    | CstI 1, y -> y
    | x, y -> Mul (simplify x, simplify y)

let rec diff x (e:aexpr) =
  match e with
  | CstI _ -> CstI 0
  | Var  v -> if v = x then  CstI 1 else CstI 0  
  | Add(e1, e2) -> Add (diff x e1, diff x e2)
  | Sub(e1, e2) -> Sub (diff x e1, diff x e2)
  | Mul(e1, e2) -> Add (Mul(diff x e1, e2), Mul(e1, diff x e2))

let ex18 = Add(Add(Mul(CstI 2,Mul(Var "x",Var "x")),Var "x"),CstI 42)
let ex19 = Add(Add(Add(Mul (CstI 0, Mul (Var "x", Var "x")),Mul (CstI 2, Add (Mul (CstI 1, Var "x"), Mul (Var "x", CstI 1)))),CstI 1), CstI 0)