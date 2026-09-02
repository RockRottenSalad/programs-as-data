module Intro
//
//type environment = (string * int)
//
//type expr =
//    | CstI of int
//    | Prim of string * expr * expr
//
//let rec eval (e : expr) : int =
//    match e with
//    | CstI i -> i
//    | Prim("+", e1, e2) -> eval e1 + eval e2
//    | Prim("*", e1, e2) -> eval e1 * eval e2
//    | Prim("-", e1, e2) -> eval e1 - eval e2
//    | Prim _ -> failwith "unknown primitive"
//
//// eval where - always spits out >= 0 if result is negative
//let rec evalm (e : expr) : int =
//    match e with
//    | CstI i -> i
//    | Prim("+", e1, e2) -> eval e1 + eval e2
//    | Prim("*", e1, e2) -> eval e1 * eval e2
//    | Prim("-", e1, e2) -> max 0 (eval e1 - eval e2)
//    | Prim _ -> failwith "unknown primitive"
//

