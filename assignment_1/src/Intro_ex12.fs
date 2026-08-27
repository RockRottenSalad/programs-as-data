module ex12

(* 1.7 | 1.2.i *)
type aexpr =
    | CstI of int
    | Var of string
    | Add of aexpr * aexpr
    | Mul of aexpr * aexpr
    | Sub of aexpr * aexpr

let e1 = Mul(Var "x", Add(Var "y", CstI 3))

(* 1.7 | 1.2.ii *)

let e2 = Sub(Var "v", Add(Var "w", Var "z"))
let e3 = Mul(CstI 3, Sub(Var "v", Add(Var "w", Var "z")))
let e4 = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")))

(* 1.7 | 1.2.iii*)

let rec fmt (a : aexpr) : string =
    let parenthesis (x : string) : string = "(" + x + ")"
    match a with
    | CstI i -> string i
    | Var x -> x
    | Add(l, r) -> parenthesis (fmt l + " + "  + fmt r)
    | Mul(l, r) -> parenthesis (fmt l + " * "  + fmt r)
    | Sub(l, r) -> parenthesis (fmt l + " - "  + fmt r)

(* 1.7 | 1.2.iv *)
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

(* 1.7 | 1.2.v *)
let rec derivative (a : aexpr) (v : string) : aexpr =
    match a with
    | CstI _ -> CstI 0
    | Var x -> CstI (if x = v then 1 else 0)
    | Add(l, r) -> Add (derivative l v, derivative r v)
    | Sub(l, r) -> Sub (derivative l v, derivative r v)
    | Mul(l, r) ->  Add (Mul(derivative l v, r), Mul(l, derivative r v))


(* 1.7 | 1.3 *)
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

let cur = Sub(Var "a", Sub(Var "b", Var "c"))
let cur2 = Sub(Sub(Var "a", Var "b"), Var "c")

