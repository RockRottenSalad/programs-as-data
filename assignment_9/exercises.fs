module Exercises

// Exercise 11.1

// Regular len
let rec len xs =
    match xs with
    | [] -> 0
    | x :: xs -> 1 + len xs

// Exercise 11.1.i
// CPS lenc
let rec lenc xs (cont : int -> int) =
   match xs with
   | [] -> cont 0
   | x :: xs -> lenc xs (fun x -> 1 + cont x)



// Exercise 11.1.ii
let rec leni xs (acc : int) =
   match xs with
   | [] -> acc
   | x :: xs -> leni xs (acc + 1)



// Exercise 11.2

// Regular rev
let rec rev xs =
    match xs with
    | [] -> []
    | x :: xs -> rev xs @ [x]

// Exercise 11.2.i

let rec revc xs (cont : 'a list -> 'a list) =
    match xs with
    | [] -> cont []
    | x :: xs -> revc xs (fun v -> x :: cont v)


// Exercise 11.2.iii

let rec revi xs (acc : 'a list) =
    match xs with
    | [] -> acc
    | x :: xs -> revi xs (x :: acc)


// Exercise 11.3

// Non-continuation-passing version
let rec prod xs =
    match xs with
    | [] -> 1
    | x :: xs -> x * prod xs

// Continuation-passing version
let rec prodc xs (cont : int -> int) =
    match xs with
    | [] -> cont 1
    | x :: xs -> prodc xs (fun v -> x * cont v)


// Exercise 11.4

// Optimized prod
let rec prodc' xs (cont : int -> int) =
    match xs with
    | [] -> cont 1
    | 0 :: _ -> 0
    | x :: xs -> prodc' xs (fun v -> x * cont v)

// Tail-recursive prod with acc
let rec prodi xs (acc : int) =
    match xs with
    | [] -> acc
    | 0 :: _ -> 0
    | x :: xs -> prodi xs (x*acc)
