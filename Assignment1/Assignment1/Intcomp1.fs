module Intcomp1

type expr =
    | CstI of int
    | Var of string
    | Let of (string * expr) list * expr
    | Prim of string * expr * expr

(* Some closed expressions: *)

let e0 = Prim("+", CstI 17, Prim("+", CstI 5, CstI 7))
let e1 = Let([ ("z", CstI 17) ], Prim("+", Var "z", Var "z"))

let e2 =
    Let([ ("z", CstI 17) ], Prim("+", Let([ ("z", CstI 22) ], Prim("*", CstI 100, Var "z")), Var "z"))

let e3 = Let([ ("z", Prim("-", CstI 5, CstI 4)) ], Prim("*", CstI 100, Var "z"))

let e4 =
    Prim("+", Prim("+", CstI 20, Let([ ("z", CstI 17) ], Prim("+", Var "z", CstI 2))), CstI 30)

let e5 = Prim("*", CstI 2, Let([ ("x", CstI 3) ], Prim("+", Var "x", CstI 4)))

let e6 = Let([ ("z", Var "x") ], Prim("+", Var "z", Var "x"))

let e7 =
    Let([ ("z", CstI 3) ], Let([ ("y", Prim("+", Var "z", CstI 1)) ], Prim("+", Var "z", Var "y")))

let e8 =
    Let([ ("z", Let([ ("x", CstI 4) ], Prim("+", Var "x", CstI 5))) ], Prim("*", Var "z", CstI 2))

let e9 =
    Let([ ("z", CstI 3) ], Let([ ("y", Prim("+", Var "z", CstI 1)) ], Prim("+", Var "x", Var "y")))

let e10 =
    Let([ ("z", Prim("+", Let([ ("x", CstI 4) ], Prim("+", Var "x", CstI 5)), Var "x")) ], Prim("*", Var "z", CstI 2))

let rec lookup env x =
    match env with
    | [] -> failwith (x + " not found")
    | (y, v) :: r -> if x = y then v else lookup r x

let rec eval e (env: (string * int) list) : int =
    match e with
    | CstI i -> i
    | Var x -> lookup env x
    | Let(vars, body) ->
        let idk = List.map (fun (x, erhs) -> (x, eval erhs env)) vars
        eval body (idk @ env)
    | Prim("+", e1, e2) -> eval e1 env + eval e2 env
    | Prim("*", e1, e2) -> eval e1 env * eval e2 env
    | Prim("-", e1, e2) -> eval e1 env - eval e2 env
    | Prim _ -> failwith "unknown primitive"

let run e = printf "%d\n" (eval e [])
let res = List.map run [ e1; e2; e3; e4; e5; e7 ] (* e6 has free variables *)


let rec mem x vs =
    match vs with
    | [] -> false
    | v :: vr -> x = v || mem x vr


let rec union (xs, ys) =
    match xs with
    | [] -> ys
    | x :: xr -> if mem x ys then union (xr, ys) else x :: union (xr, ys)

let rec minus (xs, ys) =
    match xs with
    | [] -> []
    | x :: xr -> if mem x ys then minus (xr, ys) else x :: minus (xr, ys)

let rec freevars e : string list =
    match e with
    | CstI _ -> []
    | Var x -> [ x ]
    | Let(exps, body) ->
        let occuredVariables, free =
            List.fold
                (fun (occuredVariables, free) (x, erhs) ->
                    let f' = minus (freevars erhs, occuredVariables) in (x :: occuredVariables, f' @ free))
                ([], [])
                exps

        union (free, minus (freevars body, occuredVariables))
    | Prim(_, e1, e2) -> union (freevars e1, freevars e2)

let closed2 e = (freevars e = [])
let _ = List.map closed2 [ e1; e2; e3; e4; e5; e6; e7; e8; e9; e10 ]


type texpr =
    | TCstI of int
    | TVar of int 
    | TLet of texpr * texpr
    | TPrim of string * texpr * texpr


let rec getindex vs x =
    match vs with
    | [] -> failwith "Variable not found"
    | y :: yr -> if x = y then 0 else 1 + getindex yr x


let rec tcomp (e: expr) (cenv: string list) : texpr =
    match e with
    | CstI i -> TCstI i
    | Var x -> TVar(getindex cenv x)
    | Let(exps, body) ->
        let rec idk env =
            function
            | [] -> tcomp body env
            | (x, erhs) :: rest -> TLet(tcomp erhs env, idk (x :: env) rest)

        idk [] exps
    | Prim(ope, e1, e2) -> TPrim(ope, tcomp e1 cenv, tcomp e2 cenv)

let rec teval (e: texpr) (renv: int list) : int =
    match e with
    | TCstI i -> i
    | TVar n -> List.item n renv
    | TLet(erhs, ebody) ->
        let xval = teval erhs renv
        let renv1 = xval :: renv
        teval ebody renv1
    | TPrim("+", e1, e2) -> teval e1 renv + teval e2 renv
    | TPrim("*", e1, e2) -> teval e1 renv * teval e2 renv
    | TPrim("-", e1, e2) -> teval e1 renv - teval e2 renv
    | TPrim _ -> failwith "unknown primitive"