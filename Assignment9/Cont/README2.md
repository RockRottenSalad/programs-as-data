# 11.8
> (i)
> 1. Write an expression that produces and prints the values 3 5 7 9. 
> 
> 2. Write an expression that produces and prints the values 21 22 31 32 41 42.

Below is an expression that produces and prints the values `3 5 7 9`:
```icon
Every (Write(Or(Or(Or(CstI 3, CstI 5),CstI 7),CstI 9)))
```
Running this yields:
```fsharp
> run (Every (Write(Or(Or(Or(CstI 3, CstI 5),CstI 7),CstI 9))));;
3 5 7 9
val it: value = Int 0
```

Below is an expression that yields `21, 22, 31, 32, 41, 42`:
```fs
Every(Write(Or(Or(FromTo(21, 22), FromTo(31,32)), FromTo(41,42))))
```
Running this yields:
```fsharp
> run (Every(Write(Or(Or(FromTo(21, 22), FromTo(31,32)), FromTo(41,42)))));;
21 22 31 32 41 42
val it: value = 0
```

> (ii) The micro-Icon language (like real Icon) has no boolean values. Instead, failure is
> used to mean false, and success means true. For instance, the less-than compar-
> ison operator (<) behaves as follows: 3 < 2 fails, and 3 < 4 succeeds (once) with
> the value 4. Similarly, thanks to backtracking, 3 < (1 to 5) succeeds twice, giv-
> ing the values 4 and 5. Use this to write an expression that prints the least multiple
> of 7 that is greater than 50.


Below is an expression representing this:
```fsharp
(Write(Prim("<", CstI 50, Prim("*", CstI 7, FromTo(1, 100)))))
```
The idea is rather simple. We generate multiples of 7 by multiplying `7` with the range `1..100`, and then we test whether `50` is less than that vlaue. If not, 
we have an error, and we get the next value of the range times 7. Below it can be seen run:

```fsharp
> run (Write(Prim("<", CstI 50, Prim("*", CstI 7, FromTo(1, 100)))));;                                                                                                                                                                     
56 
val it: value = Int 56
```

> (iii) Extend the abstract syntax with unary (one-argument) primitive functions, like
this: Extend the interpreter eval to handle such unary primitives, and define two such
primitives: (a) define a primitive sqr that computes the square x · x of its argument
x; (b) define a primitive even that fails if its argument is odd, and succeeds if it is
even (producing the argument as result). For instance, square(3 to 6) should
succeed four times, with the results 9, 16, 25, 36, and even(1 to 7) should suc-
ceed three times with the results 2, 4, 6.

Below is snippets demonstrating the wanted functionality:
```fsharp
> run (Every(Write(Prim1("sqr", FromTo(3,6)))));;
9 16 25 36 val it: value = Int 0

> run (Every(Write(Prim1("even", FromTo(1,7)))));;
2 4 6 val it: value = Int 0
```