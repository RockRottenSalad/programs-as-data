module Program

let rec len xs =
    match xs with
    | [] -> 0
    | x::xr -> 1 + len xr;;

let rec lenc xs f =
    match xs with
    | [] -> f 0
    | _::rest -> lenc rest (fun v -> f (v+1))

let rec leni xs acc =
    match xs with
    | [] -> acc
    | _::xr -> leni xr (acc+1);;

let rec rev xs =
    match xs with
    | [] -> []
    | x::xr -> rev xr @ [x];;

let rec revc xs f =
    match xs with
    | [] -> []
    | x::[] -> f [x]
    | x::xr -> revc xr (fun v -> f (v @ [x]))

let rec revi xs acc =
    match xs with
    | [] -> acc
    | x::xr -> revi xr (x :: acc)

let rec prod xs =
    match xs with
    | [] -> 1
    | 0::xr -> 0
    | x::xr -> x * prod xr;; 

let rec prodc xs f =
    match xs with
    | [] -> f 1
    | x::xr -> prodc xr (fun y -> x * f y);;

let rec prodi xs acc =
    match xs with
    | [] -> acc
    | 0::_ -> 0
    | x::xr -> prodi xr (acc * x);;
