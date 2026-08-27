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
  | If of expr * expr * expr (* 1.1.iv *)

(* 1.1.ii *)

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;

(* 1.1.ii *)
let e4 = Prim("min", Prim("*", Var "a", CstI 2), Var "a");;
let e5 = Prim("max", Prim("*", Var "a", CstI 2), Var "a");;

let e6 = Prim("==", Prim("*", Var "a", CstI 2), Var "a");;
let e7 = Prim("==", Var "a", Var "a");;

(* Evaluation within an environment *)

(* 1.1.3 + 1.1.4 + 1.1.5*)
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


(* 1.2.i *)
type aexpr =
    | CstI of int
    | Var of string
    | Add of aexpr * aexpr
    | Mul of aexpr * aexpr
    | Sub of aexpr * aexpr

let e1' = Mul(Var "x", Add(Var "y", CstI 3))

(* 1.2.ii *)

let e2' = Sub(Var "v", Add(Var "w", Var "z"))
let e3' = Mul(CstI 3, Sub(Var "v", Add(Var "w", Var "z")))
let e4' = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")))

(* 1.2.iii*)

let rec fmt (a : aexpr) : string =
    let parenthesis (x : string) : string = "(" + x + ")"
    match a with
    | CstI i -> string i
    | Var x -> x
    | Add(l, r) -> parenthesis (fmt l + " + "  + fmt r)
    | Mul(l, r) -> parenthesis (fmt l + " * "  + fmt r)
    | Sub(l, r) -> parenthesis (fmt l + " - "  + fmt r)

(* 1.2.iv *)
let rec simplify (a : aexpr) : aexpr =
    match a with
    | CstI _ -> a
    | Var _ -> a
    | Add(l, r) -> let l' = simplify l
                   let r' = simplify r
                   match l', r' with
                   | CstI 0, _ -> r'
                   | _, CstI 0 -> l'
                   | _ -> Add (l', r')
    | Sub(l, r) -> let l' = simplify l
                   let r' = simplify r
                   match l', r' with
                   | _, CstI 0 -> l'
                   | _ -> Sub (l', r')
    | Mul(l, r) -> let l' = simplify l
                   let r' = simplify r
                   match l', r' with
                   | CstI 0, _ -> CstI 0
                   | _, CstI 0 -> CstI 0
                   | CstI 1, _ -> r'
                   | _, CstI 1 -> l'
                   | _ -> Mul (l', r')

(* 1.2.v *)
let rec derivative (a : aexpr) (v : string) : aexpr =
    match a with
    | CstI _ -> CstI 0
    | Var x -> CstI (if x = v then 1 else 0)
    | Add(l, r) -> Add (derivative l v, derivative r v)
    | Sub(l, r) -> Sub (derivative l v, derivative r v)
    | Mul(l, r) ->  Add (Mul(derivative l v, r), Mul(l, derivative r v))



let cur = Sub(Var "a", Sub(Var "b", Var "c"))
let cur2 = Sub(Sub(Var "a", Var "b"), Var "c")


(* 1.3 *)
let fmt' (a : aexpr) : string =
    let parenthesis (x : string) : string = "(" + x + ")"
    let add_precedence = 1
    let sub_precedence = add_precedence
    let mul_precedence = 2

    let rec aux (precedence : int) (a : aexpr) : string =
        match a with
        | CstI i -> string i
        | Var x -> x
        | Add(l, r) -> let str = aux (add_precedence-1) l + " + " + aux add_precedence r
                       if precedence > add_precedence
                       then parenthesis str
                       else str
        | Sub(l, r) -> let str = aux (sub_precedence-1) l + " - " + aux sub_precedence r
                       if precedence >= sub_precedence
                       then parenthesis str
                       else str
        | Mul(l, r) -> let str = aux (mul_precedence-1) l + " * " + aux mul_precedence r
                       if precedence > mul_precedence
                       then parenthesis str
                       else str
    aux 0 a
