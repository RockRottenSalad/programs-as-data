
# Exercise 4.5
> Extend the (untyped) functional language with infix operator "&&" meaning sequential logical "and" and infix operator "||" meaning sequential logical "or", as in C, C++, Java, C#, or F#.
> Note that `e1 && e2` can be encoded as `if e1 then e2 else false` and that `e1 || e2` can be encoded as `if e1 then true else e2`.
> Hence you need only change the lexer and parser specifications, and make the new rules in the parser specification generate the appropriate abstract syntax. You need not change `Absyn.fs` or `Fun.fs`

All work here is done in the `Fun` directory.

First we match the strings "&&" and "||" to the tokens `AND` and `OR` respectively.
This is done in `FunLex.fsl`.
```fsharp
| "||"            { OR } (* CHANGED | 4.5 *)
| "&&"            { AND } (* CHANGED | 4.5 *)
```

Now `AND` and `OR` are defined as tokens in `FunPar.fsy`

```fsharp
%token AND OR /* CHANGED | 4.5 */
```

We also need to give them the lowest precedence to ensure the parsing is done correctly. This is also done in `FunPar.fsy`.

```fsharp
/* lowest precedence  */
%left AND OR            /* CHANGED | 4.5 */
%left ELSE              
%left EQ NE 
%left GT LT GE LE
%left PLUS MINUS
%left TIMES DIV MOD 
%nonassoc NOT           /* highest precedence  */
```


Finally we add two rules in `FunPar.fsy` which use the aforementioned tokens to define what this syntax means in terms of the AST.
```fsharp
| Expr OR    Expr                     { If($1, CstB true, $3)     } /* CHANGED | 4.5 */
| Expr AND   Expr                     { If($1, $3, CstB false)     } /* CHANGED | 4.5 */

```

Running this, we can verify that we get what we expect.  

```
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs Fun.fs ParseAndRun.fs
```

```fsharp
> open ParseAndRun;;
> fromString "4 = 4 || 1 < 2";;
val it: Absyn.expr =
  If (Prim ("=", CstI 4, CstI 4), CstB true, Prim ("<", CstI 1, CstI 2))

> fromString "4 = 4 && 1 < 2";;
val it: Absyn.expr =
  If (Prim ("=", CstI 4, CstI 4), Prim ("<", CstI 1, CstI 2), CstB false)
```

# Exercise 5.7
> Extend the monomorphic tpye checker to deal with lists. Use the folowing ekstra kinds of types: `| TypL of typ`

The solution here is found inside the `TypedFun` directory.

As mandated by the exercise we added another constructor `TypL of typ` to `typ`:
```fsharp
type typ =
     ...
     | TypL of typ                         (* list, element type is typ  *)
```
To implement it completely in `TypeInference.fs` we also made changes to the following functions:
- `freeTypeVars`
- `typeToString`
- `showType`
- `unify`
- `copyType`

These changes are quite simple in nature and simpy add another arm to the match statements to handle the cases when the type is a list, so please refer to these functions for the actual changes (changes are annotated with `(* CHANGED *)`).

# Exercise 6.1
> Run the evaluator on the following four programs. Is the result of the third one as expected? Explain the result of the last one:

We ran the four programs. Here's the answers to the two questions:

## The third program: Is the result what we expected?

```
let add x = let f y = x+y in f end
in let addtwo = add 2
    in let x = 77 in addtwo 5 end
    end
end
```

When put through the interepter, this evaluates to `7`. This is what we expect,
since the `x` bound to `add x` is different from the one we assigned `77` to in
the let binding. So we correctly get the result of `2 + 5`.


## The final program explained

```
let add x = let f y = x+y in f end in add 2 end
```


Here `add` is a function which takes an int `x` and returns a function `f`
which takes an int `y` and returns the result of `x + y`.
I.e. the signature for `add` would be `int -> (int -> int)`.

However, we provide `add` an argument, namely `2`. Since this is a curried
function, we just get back the function `f` with `x` being substituted for `2`.
I.e. this creates a function which takes a single integer `y` and adds `2` to
it.

Or simpler terms; this is equivalent to `let f y = 2+y in f end`.



# Exercise 6.2
> Add anonymous functions, similar to F#'s `fun x -> ...`, to the micro-ML higher-order functional language abstract syntax

First we modify `Absyn.fs` to accomodate for Lambdas
```fsharp
type expr =
...
| Fun of string * expr (* CHANGED | 6.2 *)
...
```

Then we modify `value` in `HigherFun.fs` to create a closure type for the lambda.
```fsharp
type value =
...
| Clos of string * expr * value env       (* CHANGED | 6.2 *)
...
```

Finally, we add a case to handle the lambda in the `eval` function.
```fsharp
let rec eval (e : expr) (env : value env) : value =
...
| Fun(x, fBody) -> Clos(x, fBody, env)
...
```


# Exercise 6.3 
> Extend the micro-ML lexer and parser specifications in `FunLex.fsl` and `FunPar.fsy` to permit anonymous functions.
> The concrete syntax may be as in F#: `fun x -> expr` or as in Standard ML: `fn x => expr`, where `x` is a variable.
> THe micro-ML


First we added `fun` as a keyword by modifying `FunLex.fsl`.
```fsharp
let keyword s =
...
| "fun"   -> FUN (* CHANGED | 6.2 *)
...
```

We also defined `->` as a token in `FunLex.fsl`.
```fsharp
rule Token = parse
...
| "->"            { ARROW } (* CHANGED | 6.2 *)
...
```

Then for `FunPar.fsy`, we first register the two aforementioned tokens.
```fsharp
%token FUN ARROW /* CHANGED | 6.2 */
```

Then finally we can define how a lambda function should be parsed.

```fsharp
Expr:
...
| FUN NAME ARROW Expr                 { Fun($2, $4) }  /* CHANGED | 6.2 */
...
```

Now we verify that everything works as intended:


```
dotnet build # to generate the parser/lexer
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs FunPar.fs FunLex.fs Parse.fs HigherFun.fs ParseAndRunHigher.fs
```

```fsharp
> open ParseAndRunHigher;;
> fromString "fun x -> 2*x";;
val it: Absyn.expr = Fun ("x", Prim ("*", CstI 2, Var "x"))

> fromString "let y = 22 in fun z -> z+y end";;
val it: Absyn.expr =
  Let ("y", CstI 22, Fun ("z", Prim ("+", Var "z", Var "y")))

> eval (fromString "fun x -> 2*x") [];;
val it: HigherFun.value = Clos ("x", Prim ("*", CstI 2, Var "x"), [])

> eval (fromString "let y = 22 in fun z -> z+y end") [];;
val it: HigherFun.value =
  Clos ("z", Prim ("+", Var "z", Var "y"), [("y", Int 22)])


> fromString "let add x = fun y -> x+y in add 2 5 end";;
val it: Absyn.expr =
  Letfun
    ("add", "x", Fun ("y", Prim ("+", Var "x", Var "y")),
     Call (Call (Var "add", CstI 2), CstI 5))

> fromString "let add = fun x -> fun y -> x+y in add 2 5 end";;
val it: Absyn.expr =
  Let
    ("add", Fun ("x", Fun ("y", Prim ("+", Var "x", Var "y"))),
     Call (Call (Var "add", CstI 2), CstI 5))

> eval (fromString "let add x = fun y -> x+y in add 2 5 end") [];;
val it: value = Int 7

> eval (fromString "let add = fun x -> fun y -> x+y in add 2 5 end") [];;
val it: value = Int 7

```
