# Exercise 3.5
> Get expr.zip from the book homepage and unpack it. Using a command prompt, generate (1) the lexer and (2) the parser for expressions by running fslex and fsyacc;
> then (3) load the expression abstract syntax, the lexer and parser modules, and the expression interpreter and compilers, into an interactive F# session (fsi). 
> Now try the parser on several example expressions, both well-formed and ill-formed ones, such as these, and some of your own invention:

Below are some of our own inventions.

Well-formed expressions:
```fsharp
open Parse;;
fromString "10 * let x = 25 in x * 2 end";
fromString "4 - 0 * 2";;
fromString "let x = (5*5) in let y = x*x in let z = y*y in z*z end end end";;
```

Ill-formed expressions;
```fsharp
open Parse;;
fromString "++-5";
fromString "let (x, y) = (1, 2) in x + y end";;
fromString "let x = 10 in x ** 2 end";;
```


# Exercise 3.6
> Use the expression parser from Parse.fs and the compiler `scomp` (from expressions to stack machine instructions) and the associated datatypes from Expr.fs, to define a function > `compString : string -> sinstr` list that parses a string as an expression and compiles it to stack machine code

The solution is quite simple as it simple parses the string as an `expr` using `Parse.fromString`. This `expr` is then given to the `scomp` compiler, which compiles to a list of `sinstr`. The solution can be seen below:

```fsharp
(* parse expression in string form to expr and compiles to stack machine byte code using scomp *)
let compString (source: string) : sinstr list =
    let e = Parse.fromString source
    scomp e []
```
It can naturally also be found in `Expr/Expr.fs` on line `344`.


# Exercise 3.7 - Extend the expression language abstract syntax and the lexer and parser specifications with conditions expressions. The abstract syntax should be If(e1, e2, e3)...

Use grep(or something else that can search) and look for `CHANGED | 3.7` to find all these changes.

# Exercise 4.1
> Get archive fun.zip from the homepage and unpack to directory `Fun`. It contains lexer and parser specifications and interpreter for a small first-order functional language. |> Generate and compile the lexer and parser as described in `README.TXT`; parse and run some example programs with `ParseAndRun.fs`.

This has been done, nothing work mentioning here.

# Exercise 4.2
> Write more example programs in the functional language, and test them in the same way as in Exercise 4.1
> 1. Computer the sum of the numbers from 1000 down to 1. Do this by defining a function `sum n` that computes the sum `n + (n - 1) + ... + 2 + 1.`

```
let sum n = if n < 1 then 0 else n + sum (n-1) in sum 1000 end
```

> Compute the number 3^8, that is, 3 raised to the power 8. Again, use a recursive function.
> 
```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in powerOf3 8 end
```

> Compute `3^0 + 3^1 + ... + 3^10 + 3^11`, using a recursive function (or two, if you prefer)
> 
```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in 
let sumPowersOf3 n = if n = 0 then 1 else sumPowersOf3 (n-1) + powerOf3 n in
sumPowersOf3 11
end end
```

> Compute `1^8 + 2^8 + ... + 10^8`, again using a recursive function (or two).
> 
```
let eigthPower n = n*n*n*n*n*n*n*n in 
let sumRaisedToEigth n = if n = 1 then 1 else sumRaisedToEigth (n-1) + eigthPower n in
sumRaisedToEigth 10
end end
```

# Exercise 4.3
> Modify the language to allow functions to take one or more arguments.

**Note:** For all of these, please do look at the actual files. Below is a summary of the changes made to solve the exercise.

The first change was modifying the abstract syntax in `Absyn.fs` to allow it to take more than one argument.
```fsharp
Letfun of string * string list * expr * expr
Call of expr * expr list
```
As seen above, this was done by making `Letfun` accept a list of parameter names (`string`s) and making `Call` accept a list of arguments (`expr`s).

The second change was modyifing the `Closure` representation to store a list of function parameters rather than only one:
```fsharp
Closure of string * string list * expr * value env
```
What this means in practice is that a closure is essentially `Closure(f, params, fBody, fDeclEnv)`.

To finally integrate this into the interpreter the `env` function had to be changed, specifically `Call` branch in order to support the new abstract syntax.
- `Call` was chan



# Exercise 4.4 (Functions that support multiple argument)

Use grep(or something else that can search) and look for `CHANGED | 4.3` and
`CHANGED | 4.4` to find all these changes.

