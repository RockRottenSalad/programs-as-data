(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [ ("a", 3); ("c", 78); ("baf", 666); ("b", 111) ]

let emptyenv = [] (* the empty environment *)

let rec lookup env x =
    match env with
    | [] -> failwith (x + " not found")
    | (y, v) :: r -> if x = y then v else lookup r x

let cvalue = lookup env "c"


(* Object language expressions with variables *)

type expr =
    | CstI of int
    | Var of string
    | Prim of string * expr * expr
    | If of expr * expr * expr

let e1 = CstI 17

let e2 = Prim("+", CstI 3, Var "a")

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a")


(* Evaluation within an environment *)

(*1.1.i*)
(*1.1.iii*)
let rec eval e (env: (string * int) list) : int =
    match e with
    | CstI i -> i
    | Var x -> lookup env x
    | Prim(op, e1, e2) ->
        let lhs = eval e1 env
        let rhs = eval e2 env

        match op with
        | "+" -> lhs + rhs
        | "*" -> lhs * rhs
        | "-" -> lhs - rhs
        | "max" -> max lhs rhs
        | "min" -> min lhs rhs
        | "==" -> if lhs = rhs then 1 else 0
        | _ -> failwith "unknown primitive"
    | If(e1, e2, e3) -> if eval e1 env <> 0 then eval e2 env else eval e3 env

(*1.1.ii*)
let a = eval (Prim("max", Prim("+", e1, e2), CstI 3)) [ ("a", 314) ]
let b = eval (Prim("min", Prim("+", e1, e2), CstI 3)) [ ("a", 314) ]
let c = eval (Prim("==", CstI 3, CstI 3)) []
let d = eval (Prim("==", CstI 4, CstI 3)) []

(* Exercise 1.2 *)
(* 1.2.i *)
type aexpr =
    | CstI of int
    | Var of string
    | Add of aexpr * aexpr
    | Mul of aexpr * aexpr
    | Sub of aexpr * aexpr

(* 1.2.ii *)
let e = Sub(Var "v", Add(Var "w", Var "z")) // v − (w + z)
let f = Mul(CstI 2, Sub(Var "v", Add(Var "w", Var "z"))) // 2 * (v - (w + z))
let g = Add(Var "x", Add(Var "y", Add(Var "z", Var "v"))) // x + y + z + v

(* 1.2.iii *)
let binop op a b = a + " " + op + " " + b
let parenthesize a = "(" + a + ")"

let rec fmt =
    function
    | CstI x -> string x
    | Var x -> x
    | Add(e1, e2) -> binop "+" (fmt e1) (fmt e2) |> parenthesize
    | Mul(e1, e2) -> binop "*" (fmt e1) (fmt e2) |> parenthesize
    | Sub(e1, e2) -> binop "-" (fmt e1) (fmt e2) |> parenthesize

(*1.2.iv *)
let rec simplify =
    function
    | CstI x -> CstI x
    | Var x -> Var x
    | Add(e1, e2) ->
        match simplify e1, simplify e2 with
        | CstI 0, y -> y
        | x, CstI 0 -> x
        | x, y -> Add(x, y)
    | Sub(e1, e2) ->
        match simplify e1, simplify e2 with
        | x, CstI 0 -> x
        | x, y when x = y -> CstI 0
        | x, y -> Sub(x, y)
    | Mul(e1, e2) ->
        match simplify e1, simplify e2 with
        | CstI 0, _ -> CstI 0
        | _, CstI 0 -> CstI 0
        | CstI 1, y -> y
        | x, CstI 1 -> x
        | x, y -> Mul(x, y)

(* 1.2.v *)
let rec sym_dif x =
    function
    | CstI _ -> CstI 0
    | Var y -> if x = y then CstI 1 else CstI 0
    | Add(e1, e2) -> Add(sym_dif x e1, sym_dif x e2)
    | Mul(e1, e2) -> Add(Mul(sym_dif x e1, e2), Mul(e1, sym_dif x e2))
    | Sub(e1, e2) -> Sub(sym_dif x e1, sym_dif x e2)