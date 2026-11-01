# 11.1

> (i) Write a continuation-passing (CPS) version `lenc` : ’a list -> (int -> ’b) -> ’b of the list length function len

Here it is:
```F#
let rec lenc xs f =
    match xs with
    | [] -> f 0
    | first::rest -> lenc rest (fun v -> f (v+1))
```
>Try calling the resulting function with `lenc` [2; 5; 7] id and with `lenc` [2; 5; 7] (printf "The answer is ’%d’\n")

I'll open fsi first:
```shell
dotnet fsi Program.fs
```
then open the right module, and run it like so:
```F#
> open Program;;
> lenc [2; 5; 7] id;;
val it: int = 3

> lenc [2; 5; 7] (printf "The answer is ’%d’\n");;
The answer is ’3’
val it: unit = ()
```
> (ii) What happens if you call it as `lenc` xs (fun v -> 2*v) instead?

The function given when `lenc` is called executes when base case is true so, the final result doubles, like so:
```F#
> lenc [2;5;7] (fun v -> 2*v);;
val it: int = 6
```

> (iii) Write also a tail-recursive version leni. What is the relation between `lenc` and `leni`?

this is `leni`:
```F#
let rec leni xs acc =
    match xs with
    | [] -> acc
    | _::xr -> leni xr (acc+1);;
```
We love `leni`. `lenc` introduces a trade off between stack and heap, uses more heap space but less stack space by using continuations, compared to `len`. `leni`, however uses both less stack and heap space, because it uses an accumulator. 
# 11.2
> (i) Write a continuation-passing version `revc` of the list reversal function rev
Here it is:
```f#
let rec revc xs f =
    match xs with
    | [] -> []
    | x::[] -> f [x]
    | x::xr -> revc xr (fun v -> f (v @ [x]))
```

```F#
>revc [1;2;3] (fun v -> v @ v);;
val it: int list = [3; 2; 1; 3; 2; 1]
```
> (ii) What happens if you call it as `revc xs (fun v -> v @ v)` instead?

The function given when `revc` is called executes when base case is true so, the resulting list appends itself to itself like so:
```F#
>revc [1;2;3] (fun v -> v @ v);;
val it: int list = [3; 2; 1; 3; 2; 1]
```

>(iii) Write a tail-recursive reversal function `revi` [...].

Done:
```f#
let rec revi xs acc =
    match xs with
    | [] -> acc
    | x::xr -> revi xr (x :: acc)
```


# 11.3
>Write a continuation-passing version `prodc` [...]

here it is:
```F#
let rec prodc xs f =
    match xs with
    | [] -> f 1
    | x::xr -> prodc xr (fun y -> x * f y);;
```
We can test it too, to see if it works:
```F#
> open Program;;
> prodc [1;2;3] id;;
val it: int = 6
```
# 11.4
>Optimize the CPS version of the `prod` function above. [...]
here it is:
```F#
let rec prod xs =
    match xs with
    | [] -> 1
    | 0::xr -> 0
    | x::xr -> x * prod xr;; 
```
We can test it too, to see if it works:
```F#
> prod [1;2;3];;   
val it: int = 6
```

> [...] Write a tail-recursive version `prodi` of the `prod` function that also terminates as soon as it encounters a zero in the list.

Here it is:
```F#
let rec prodi xs acc =
    match xs with
    | [] -> acc
    | 0::_ -> 0
    | x::xr -> prodi xr (acc * x);;
```
We can test it too, to see if it works:
```F#
> prodi [1;2;3] 1;; 
val it: int = 6
```
# 11.8
>(i) Write an expression that produces and prints the values `3 5 7 9`. [...]
```F#
let myex1 = Every(Write(Or(Or(Or(CstI 3, CstI 5), CstI 7), CstI 9)));
```
> [...] Write an expression that produces and prints the values `21 22 31 32 41 42`

```F#
let myex2 = Every(Write(Or(Or(Or(Or(Or(CstI 21, CstI 22), CstI 31), CstI 32), CstI 41), CstI 42)));
```

> (ii) [...] Use this to write an expression that prints the least multiple of `7` that is greater than `50`.

Here is my solution:
```F#
let myex3 = Write(Prim("<", CstI 50, Prim("*", CstI 7, FromTo(1,100))));
```
In fsi:
```F#
> run myex3;;
56 val it: value = Int 56
```

> (iii) Extend the abstract syntax with unary (one-argument) primitive functions [...]

```F#
type expr = 
  ...
  | Prim1 of string * expr (* CHANGED *)
  ...
```
here is the added primitives in `Icon.fs`:
```F#
let rec eval (e : expr) (cont : cont) (econt : econt) = 
    match e with
    ...
    | Prim1(ope, e) -> (* CHANGED *)
      eval e (fun v1 -> fun econt1 ->
      match (ope, v1) with
      | "sqr", Int i ->
          cont (Int(i*i)) econt1
      | "even", Int i ->
        if i % 2 = 0 then cont (Int i) econt1 else econt1 ()
```
here are some examples to test `eval` with the new changes:
```F#
let myex4 = Every(Write(Prim1("sqr", FromTo(3, 6))));

let myex5 = Every(Write(Prim1("even", FromTo(1, 7))));
```
In fsi:
```F#
> run myex4;;
9 16 25 36 val it: value = Int 0

> run myex5;;
2 4 6 val it: value = Int 0
```

> (iv) Define a unary primitive multiples that succeeds infinitely many times, producing all multiples of its argument.

here is the added primitives in `Icon.fs`:
```F#
let rec eval (e : expr) (cont : cont) (econt : econt) = 
    match e with
    ...
    | Prim1(ope, e) -> (* CHANGED *)
      eval e (fun v1 -> fun econt1 ->
      match (ope, v1) with
      ...
      | "mul", Int i ->
        let rec loop i = 
          cont (Int i) (fun () -> loop (i+1))
        loop i
```
here is an example to test `eval` with the new changes:
```F#
let myex6 = Every(Write(Prim1("mul", CstI 3)));
```
In fsi:
```F#
> run myex6;;
3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 ...
```