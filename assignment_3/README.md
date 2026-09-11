# Exercise 3.5 - Get expr.zip from the book homepage and ..., Now try the parser on several example expressions, both well-formed and ill-formed ones, ..., and some of your own invetion!

Our own invetions:

Some well formed expressions:

```
open Parse;;
fromString "10 * let x = 25 in x * 2 end";
fromString "4 - 0 * 2";;
fromString "let x = (5*5) in let y = x*x in let z = y*y in z*z end end end";;
```

Some ill-formed expressions:

```
open Parse;;
fromString "++-5";
fromString "let (x, y) = (1, 2) in x + y end";;
fromString "let x = 10 in x ** 2 end";;
```


# Exercise 3.6 - Use the expression parser from Parse.fs and the compiler scompo and the associated datatypes from Expr.fs, to define a function compString: string -> sinstr list.

See `Expr/Expr.fs` on line `344`.

```fsharp
(* parse expression in string form to expr and compiles to stack machine byte code using scomp *)
let compString (source: string) : sinstr list =
    let e = Parse.fromString source
    scomp e []
```

# Exercise 3.7 - Extend the expression language abstract syntax and the lexer and parser specifications with conditions expressions. The abstract syntax should be If(e1, e2, e3)...

Use grep(or some else that can search) and look for `CHANGED | 3.7` to find all these changes.


# Exercise 4.2 - Write more example progrms in the functional language, and test them in the same way as in Exercise 4.1

- Computer the sum of the numbers from 1000 down to 1. Do this by defining a function `sum n` that computes the sum `n + (n - 1) + ... + 2 + 1.`

```
let sum n = if n < 1 then 0 else n + sum (n-1) in sum 1000 end
```

- Compute the number 3^8, that is, 3 raised to the power 8. Again, use a recursive function.

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in powerOf3 8 end
```

- Compute `3^0 + 3^1 + ... + 3^10 + 3^11`, using a recursive function (or two, if you prefer)

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in 
let sumPowersOf3 n = if n = 0 then 1 else sumPowersOf3 (n-1) + powerOf3 n in
sumPowersOf3 11
end end
```

- Compute `1^8 + 2^8 + ... + 10^8`, again using a recursive function (or two).


```
let eigthPower n = n*n*n*n*n*n*n*n in 
let sumRaisedToEigth n = if n = 1 then 1 else sumRaisedToEigth (n-1) + eigthPower n in
sumRaisedToEigth 10
end end
```


# Exercise 4.3 & Exercise 4.4 (Functions that support multiple argument)

Use grep(or some else that can search) and look for `CHANGED | 4.3` and
`CHANGED | 4.4` to find all these changes.

