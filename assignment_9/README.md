
# Exercise 11.1

All of the referenced code for these exercises can be found in the file called `exercises.fs`.

> (i) Write a continuation-passing (CPS) version `lenc: 'a list -> (int -> 'b) -> 'b` of the list length function `len`

The idea here is that we recursively construct a function that
adds `1` for each element in the list.

```fsharp
let rec lenc xs (cont : int -> int) =
   match xs with
   | [] -> cont 0
   | x :: xs -> lenc xs (fun x -> 1 + cont x)
```

> (ii) What happens if you call it as `lenc xs (fun v -> 2*v)`

With our implementation, it still works as intended. The provided intial continuation function is applied with the argument `0`, and since `2*0 = 0`, we add `0` to the final result.

```fsharp
lenc [1;2] (fun v -> 2*v)
lenc [2] (fun x -> 1 + (fun v -> 2*v) x )
lenc [] (fun x -> 1 + (fun x -> 1 + (fun v -> 2*v) x) x )
(fun x -> 1 + (fun x -> 1 + (fun v -> 2*v) x) x ) 0
(1 + (fun x -> 1 + (fun v -> 2*v) x) 0 )
(1 + (1 + (fun v -> 2*v) 0) )
(1 + (1 + (2*0)) )
(1 + (1 + 0) )
(1 + 1)
2
```

> (iii) Write also a tail-recursive version `leni : int list -> int -> int` of the length function, whose second parameter is an accumulating parameter. The function should be called as `leni xs 0`. What is the relation between `lenc` and `leni`?

We simply call the function recursively and add 1 to the accumulator in each recursive call.

```fsharp
let rec leni xs (acc : int) =
   match xs with
   | [] -> acc
   | x :: xs -> leni xs (acc + 1)

```

The relation between `leni` and `lenc` is that both fuctions are tail-recursive, unlike the original `len` function.


# Exercise 11.2

> (i) Write a continuation-passing version `revc` of the list reversal function `rev`:

For each recursive call, we simply construct a new
continuation function that appends each element to the front
of whatever the continuation fuction returns. This will
effectively reverse the list.

```fsharp
let rec revc xs (cont : 'a list -> 'a list) =
    match xs with
    | [] -> cont []
    | x :: xs -> revc xs (fun v -> x :: cont v)
```

> (ii) What happens if you call it as `revc xs (fun v -> v @ v)` instead?

In our implementation, nothing changes. It still works as
expected. This is because the intially given continuation
function will be applied with the argument `[]`, i.e. we're
concatting two empty lists together, which has no effect.

```fsharp
revc [1;2] (fun v -> v @ v)
revc [2] (fun v -> 1 :: (fun v -> v @ v) v)
revc [] (fun v -> 2 :: (fun v -> 1 :: (fun v -> v @ v) v) v)
(fun v -> 2 :: (fun v -> 1 :: (fun v -> v @ v) v) v) []
2 :: (fun v -> 1 :: (fun v -> v @ v) v) []
2 :: 1 :: (fun v -> v @ v) []
2 :: 1 :: ([] @ [])
2 :: 1 :: []
2 :: [1]
[2; 1]
```

> (iii) Write a tail-recursive reversal function `revi : 'a list -> 'a list -> 'a list`, whose seccond parameter is an accumulating parameter, and which should be called as `revi xs[]`.

We simply append each element of the list to the front of the accumulator.

```fsharp
let rec revi xs (acc : 'a list) =
    match xs with
    | [] -> acc
    | x :: xs -> revi xs (x :: acc)
```

# Exericse 11.3

> Write a continuation-passing version `prodc : int list -> (int -> int) -> int` of the list product function prod

At each recursive call, we make a new continuation function which
multiplies the current element with whatever is in the current
continuation.

```fsharp
let rec prodc xs (cont : int -> int) =
    match xs with
    | [] -> cont 1
    | x :: xs -> prodc xs (fun v -> x * cont v)
```

# Exercise 11.4

> Optimize the CPS version of the `prod` function above. It could termiante as soon as it encounters a zero in the list, assuming that its continuation simply multiplies the result by some factor.

We simply add another pattern match that checks if the element is `0`, in which case we immediately just return `0`.

```fsharp
let rec prodc' xs (cont : int -> int) =
    match xs with
    | [] -> cont 1
    | 0 :: _ -> 0
    | x :: xs -> prodc' xs (fun v -> x * cont v)
```

> Try calling it in the same two ways as the `lenc` function in `Exercise 11.1

Calling the function as `prodc' [1;2;3] id` correctly yields 6.

However, this time calling the function incorrectly like so `prodc' [1;2;3] (fun v -> 2*v)` will incorrectly yield `12`. This is because the final result is effectively multiplied by `2`.

> Write a tail-recursive version `prodi` of the `prod` function that also termiantes as soon as it encounters a zero in the list.

Same as `prodc'`, just with an accumulator instead of a continuation.

```fsharp
let rec prodi xs (acc : int) =
    match xs with
    | [] -> acc
    | 0 :: _ -> 0
    | x :: xs -> prodi xs (x*acc)
```

# Exercise 11.8

> (i) Write an expression that produces and prints the values `3 5 7 9`.

We simply use the `Or` AST node to build an expression that yields the expected numbers and then use `Every` to force all the results.

```fsharp
Every(Write(Or(
            CstI 3,
            Or(CstI 5, 
                Or(CstI 7, CstI 9)
              )
          )
))
```

And now we can run it and see the result is what we expect:

```fsharp
> run (Every(Write(Or(
-             CstI 3,
-             Or(CstI 5,
-                 Or(CstI 7, CstI 9)
-               )
-           )
- ))
- );;
3 5 7 9 val it: value = Int 0
```

> Write an expression that produces and prints the values `21 22 31 32 41 42`

Like before we chain together a bunch of `Or`s, but this time we can utilise the fact that some of the numbers that we need to print out are successors of the prior number, thus we can simplify our code by using `FromTo`. I.e. we use `FromTo(21, 22)` to produce `21 22`.

```fsharp
Every(Write(Or(
            FromTo(21, 22),
            Or(FromTo(31, 32), FromTo(41, 42))
)))
```

Running this we get:

```fsharp
> run (Every(Write(Or(
-             FromTo(21, 22),
-             Or(FromTo(31, 32), FromTo(41, 42))
- )))
- );;
21 22 31 32 41 42 val it: value = Int 0
```

> (ii) The micro-Icon language (like real Icon) has no boolean values. Instead, failure is used to mean false, and success means true. For instance, the less-than comparison operator (<) behaves as follows: 3 < 2 fails, and 3 < 4 succeeds (once) with the value 4. Similarly, thanks to backtracking, 3 < (1 to 5) succeeds twice, giving the values 4 and 5. Use this to write an expression that prints the least multiple of 7 that is greater than 50.


The idea is we first generate the first 10 multiples of 7 like so:

```fsharp
Prim("*", FromTo(1, 10), CstI 7)
```

Now we simply perform a `50 < ` comparison on the range generated above and write out the result.

```fsharp
Write(Prim("<", CstI 50, Prim("*", FromTo(1, 10), CstI 7)))
```

Running this we get:

```fsharp
> run (Write(Prim("<", CstI 50, Prim("*", FromTo(1, 10), CstI 7))));;
56 val it: value = Int 56
```

> (iii) Extend the abstract syntax with unary (one-argument) primitive functions, like this:

```fsharp
type expr =
    | ...
    | Prim1 of string * expr 
```

> Extend the interpreter eval to handle such unary primitives

> and define two such primitives:

> (a) define a primitive `sqr` that computes the square `x · x` of its argument `x`;

> (b) define a primitive even that fails if its argument is odd, and succeeds if it is even (producing the argument as result). For instance, `square(3 to 6)` should succeed four times, with the results `9, 16, 25, 36`, and `even(1 to 7)` should succeed three times with the results `2, 4, 6`.

Firstly we update the AST to accomodate unary primitives.

```fsharp
type expr = 
  ...
  | Prim1 of string * expr (* CHANGED | Exercise 11.8 *)
  ...
```

Then we update the `eval` function to handle the unary primitives .

The `sqr` primitive simply squares the number.

The `even` primtive checks whether if the number is even or not and
decides whether to pass a value to `cont` or call the error continuation
`econt`.

```fsharp
let rec eval (e : expr) (cont : cont) (econt : econt) = 
    match e with
    ...
    | Prim1(ope, e1) -> (* CHANGED | Exercise 11.8 *)
      eval e1 (fun v1 -> fun econt1 ->
          match (ope, v1) with
          | ("sqr", Int i1) -> cont (Int(i1*i1)) econt1
          | ("even", Int i1) -> if i1 % 2 = 0
                                then cont (Int i1) econt1
                                else econt1 ()
          | _ -> Str "unknown prim1"
      ) econt
    ...
```


Now we test the `sqr` primitive with the range `FromTo(3, 6)` and see if we get what we expect:
```fsharp
> run (Every(Write(Prim1("sqr", FromTo(3, 6)))));;
9 16 25 36 val it: value = Int 0
```

And finally we also test the `even` primitive with the range `FromTo(1, 7)`
```fsharp
> run (Every(Write(Prim1("even", FromTo(1, 7)))));;
2 4 6 val it: value = Int 0
```

> (iv) Define a unary primitive multiples that succeeds infinitely many times, producing all multiples of its argument. For instance, `multiples(3)` should produce `3, 6, 9, ...` Note that `multiples(3 to 4)` would produce multiples of `3` forever, and would never backtrack to the subexpression `(3 to 4)` to begin producing multiples of `4`.

We begin by adding a match case for the unary primtive defined earlier. 

Then similar to how the `FromTo` is implemented, we define a function that
yields the current value and in the error path recurisvely generates the
next value, thus effectively creating an infinite sequence.

```fsharp
let rec eval (e : expr) (cont : cont) (econt : econt) = 
    match e with
      ...
    | Prim1(ope, e1) -> (* CHANGED | Exercise 11.8 *)
      eval e1 (fun v1 -> fun econt1 ->
          match (ope, v1) with
          ...
          | ("multiples", Int i1) ->
                let rec multiples (i : int) (acc : int) =
                    cont (Int acc) (fun () -> multiples i (i + acc))
                multiples i1 i1
          ...
      ) econt
```

We can then test this by re-implementing our program from earlier which found the first multiple of `7` that is greater than `50`.

```fsharp
Write(Prim("<", CstI 50, Prim1("multiples", CstI 7)))
```

And see that we get what we expect:

```fsharp
> run (Write(Prim("<", CstI 50, Prim1("multiples", CstI 7))));;
56 val it: value = Int 56
```

