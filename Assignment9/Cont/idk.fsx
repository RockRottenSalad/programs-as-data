let rec lenc xs c =
    match xs with
    | [] -> c 0
    | _ :: xr -> lenc xr (fun res -> c (1 + res));;
    
let rec leni xs acc =
    match xs with
    | [] -> acc
    | _ :: xr -> leni xr (acc + 1);;
    
    
let rec revc xs c =
    match xs with
    | [] -> c []
    | x::xr -> revc xr (fun res -> c (res @ [x]));;
    
let rec revi xs acc =
    match xs with
    | [] -> acc
    | x::xr -> revi xr (x :: acc);;
    
let rec prodc xs c =
    match xs with
    | [] -> c 1
    | 0 :: _ -> c 0
    | x::xr -> prodc xr (fun res -> c (x * res));;
    
let rec prodi xs acc =
    match xs with
    | [] -> acc
    | 0 :: _ -> 0
    | x::xr -> prodi xr (x * acc);;