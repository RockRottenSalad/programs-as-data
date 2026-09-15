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


# Exercise 3.7
> Extend the expression language abstract syntax and the lexer and parser specifications with conditions expressions. The abstract syntax should be If(e1, e2, e3)...

HINT: Use grep(or something else that can search) and look for `CHANGED | 3.7` to find the exact location of the changes rather than manually scrolling through the files.

In `ExprLex.fsl`, we add cases to match the different tokens that are used in
an If statement.

```fsharp
| "if" -> IF        (* CHANGED | 3.7 *)
| "then" -> THEN    (* CHANGED | 3.7 *)
| "else" -> ELSE    (* CHANGED | 3.7 *)
```

In `ExprPar.fsy` added tokens used in an If statement:

```fsharp
%token IF    /* CHANGED | 3.7 */
%token THEN  /* CHANGED | 3.7 */
%token ELSE  /* CHANGED | 3.7 */
```

Also added a rule for If states under `Expr:`. The idea is we expect an IF
token, then a boolean expression, afterwards a THEN token and finally the two
cases depending on what the boolean expression evaluates to.
```fsharp
| IF Expr THEN Expr ELSE Expr         { If($2, $4, $6)    } /* CHANGED | 3.7 */
```

In `Absyn.fs` we've defined the If statement as a part of our AST.
```fsharp
| If of expr * expr * expr
```

In `Expr.fs`, add the functions which match on an expression have been modified to accomodate for the new If statement

In the `eval` function we've simply added a case which evaluates the correct branch depending on whether if the guard was true or false.
```
| If(guard, e1, e2) -> if eval guard env <> 0 then eval e1 env else eval e2 env
```

For both `fmt1` and `fmt2`(the string formatting functions), we simply print out the if statement.
```fsharp
(* In fmt1: *)
| If(guard, e1, e2) -> String.concat " " [ "if"; fmt1 guard; "then"; fmt1 e1; "else"; fmt1 e2 ]

(* In fmt2: *)
| If(guard, e1, e2) -> String.concat " " [ "if"; fmt2 -1 guard; "then"; fmt2 -1 e1; "else"; fmt2 -1 e2 ]

```

We also modified the `closedin` function to check that the guard expression and both expressions are closed in the if statement.
```fsharp
    (* CHANGED | 3.7 *)
    | If(guard, e1, e2) -> closedin guard env && closedin e1 env && closedin e2 env

```

The `freevars` function has been modified to accomodate for the if statement. We simply recursively union together all the free vars from each expression in the if statement.

```fsharp
(* CHANGED | 3.7 *)
| If(guard, e1, e2) -> union (union (freevars guard) (freevars e1)) (freevars e2)

```

In the `tcomp` function we simply compile the expr if statement into a tcomp if statement.
```fsharp
(* CHANGED | 3.7 *)
| If(guard, e1, e2) -> TIf(tcomp guard cenv, tcomp e1 cenv, tcomp e2 cenv)
```

And naturally, since we've modified `tcomp` we also need to modify `teval` to accomodate the if statement.
This is basically the same as the regular eval, just for a `texpr` instead of `expr`.
```fsharp
| TIf(guard, e1, e2) -> if teval guard renv <> 0 then teval e1 renv else teval e2 renv
```


# Exercise 4.1
> Get archive fun.zip from the homepage and unpack to directory `Fun`. It contains lexer and parser specifications and interpreter for a small first-order functional language. |> Generate and compile the lexer and parser as described in `README.TXT`; parse and run some example programs with `ParseAndRun.fs`.

This has been done, nothing worth mentioning here.

# Exercise 4.2
> Write more example programs in the functional language, and test them in the same way as in Exercise 4.1
> 1. Computer the sum of the numbers from 1000 down to 1. Do this by defining a function `sum n` that computes the sum `n + (n - 1) + ... + 2 + 1.`

```
let sum n = if n < 1 then 0 else n + sum (n-1) in sum 1000 end
```

> 2. Compute the number 3^8, that is, 3 raised to the power 8. Again, use a recursive function.

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in powerOf3 8 end
```

> 3. Compute `3^0 + 3^1 + ... + 3^10 + 3^11`, using a recursive function (or two, if you prefer)

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in 
let sumPowersOf3 n = if n = 0 then 1 else sumPowersOf3 (n-1) + powerOf3 n in
sumPowersOf3 11
end end
```

> 4. Compute `1^8 + 2^8 + ... + 10^8`, again using a recursive function (or two).

```
let eigthPower n = n*n*n*n*n*n*n*n in 
let sumRaisedToEigth n = if n = 1 then 1 else sumRaisedToEigth (n-1) + eigthPower n in
sumRaisedToEigth 10
end end
```

# Exercise 4.3
> Modify the language to allow functions to take one or more arguments.

Use grep(or something else that can search) and look for `CHANGED | 4.3` and
`CHANGED | 4.4` to find all these changes.

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

Upon looking up the function closure in the environment, we evaluate each argument within the current environment. We then zip each evaluated argument with their associated variable name and add it to the environment.

We've left the previous code commented out so that it's easier to see the difference.
```fsharp
    | Call(Var f, eArgs) -> (* CHANGED | 4.3*)
      let fClosure = lookup env f
      match fClosure with
      | Closure (f, x, fBody, fDeclEnv) -> (* CHANGED | 4.3 *)
        let xVals = List.map (fun eArg -> eval eArg env |> Int) eArgs
        (*let xVal = Int(eval eArg env)*)
        (*let fBodyEnv = (x, xVal) :: (f, fClosure) :: fDeclEnv*)
        let fBodyEnv = List.zip x xVals @ (f, fClosure) :: fDeclEnv
        eval fBody fBodyEnv

```




# Exercise 4.4 
> In continuation of Exercise 4.3, modify the parser specification to accept a language where functions may take any (non-zero) > number of arguments. You need to modify the AppExpr nonterminal and its semantic action to produce `Call(Var "f", [Var "a"; Var > "b"])` instead.

Use grep(or something else that can search) and look for `CHANGED | 4.3` and
`CHANGED | 4.4` to find all these changes.

For `AtExpr` the `Letfun` Grammar definition changed, substituting `NAME` for `StringArgs` in `FunPar.fsy`, as follows:

```
/* CHANGED | 4.4 */
AtExpr:
    Const                                     { $1                     }
  | NAME                                      { Var $1                 }
  | LET NAME EQ Expr IN Expr END              { Let($2, $4, $6)        }
  | LET NAME StringArgs EQ Expr IN Expr END   { Letfun($2, $3, $5, $7) }
  | LPAR Expr RPAR                            { $2                     }
;
```
where `StringArgs` is defined as:
```
StringArgs: 
    NAME                                { [$1]     }
  | NAME StringArgs                     { $1 :: $2 }
;
```
The idea is substituting the terminal `NAME` with a non-terminal `StringArgs` which can either be a Terminal `NAME` or a Terminal `NAME` followed by a non-terminal `StringArgs`. This way the grammar recursively allows one of more arguments.

```
/* CHANGED | 4.4 */
AppExpr:
    AtExpr Args                         { Call($1, $2)           }
  | AppExpr Args                        { Call($1, $2)           }
;

Args: 
    AtExpr                                { [$1]     }
  | AtExpr Args                           { $1 :: $2 }
;
```
The same logic was applied for `AppExpr` with `Args`. 

