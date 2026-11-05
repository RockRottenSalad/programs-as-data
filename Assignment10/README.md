12.1

did both ifzero and ifnzero

changed bunch of places

# Exercise 12.2
> (i) Improve code generation in the continuation-based micro-C compiler
> so that a less-than comparison with constant arguments is compiled to its truth value.
> For instance, 11 < 22 should compile to the same code as true, and 22 < 11
> should compile to the same code as false. This can be done by a small extension
> of the addCST function in Contcomp.fs

If we have `CSTI i :: CST j :: LT :: c1` then we can simply evaluate `i < j` in the compiler and put either `1` or `0` on the stack. Afterwards, its worth checking if this opens up for more optimization, so we call `addCST` on this. This can be seen done below:
```fsharp
let rec addCST i c =
    ...
    | (_, CSTI j :: LT :: c1) -> addCST (if i < j then 1 else 0) c1
    ...
```
> (ii) Further improve the code generation so that all comparisons with constant argu-
> ments are compiled to the same code as true (e.g. 11 <= 22 and 11 != 22
> and 22 > 11 and 22 >= 11) or false.

To do this we have to observe the patterns that the comparison operations leave on the stack. This can be seen in `cExpr`. The solution is as follows:
```fsharp
let rec addCST i c =
    match (i, c) with
    // < and >= case
    | (_, CSTI j :: LT :: c1) -> addCST (if i < j then 1 else 0) c1 // Changed 12.2
    // <= and >
    | (_, CSTI j :: SWAP :: LT :: c1) -> addCST (if j < i then 1 else 0) c1 // Changed 12.2
    // = and !=
    | (_, CSTI j :: EQ :: c1) -> addCST (if i = j then 1 else 0) c1 // Changed 12.2
    ...
```
The important thing to note is that `!=` and `>=` adds a `not` ater their instruction, so their case is already covered by `<` and `=`. Now for `<=` and `>` it essentially just swaps the argument, so to allow for optimization, we also add a case for that.

> (iii) Check that if (11 <=22) print 33; compiles to code that unconditionally executes print 33 without performing any test or jump.

To test this we have written a sample program in `CEx/text.c`:
```c#
void main()
{
    if (11 <= 22)
    {
        print 33;
    }
}
```

Compiling this program yields:
```fsharp
> compileToFile (fromFile "CEx/test.c") "CEx/test.out";;
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; CSTI 33; PRINTI; RET 0;
   Label "L2"; RET -1]
```
Where its clear that it uncondtionally executes the print without performing any test or jump.

# Exercise 12.3
> (i) Extend the micro-C abstract syntax (file Absyn.fs) with condi-
> tional expressions Cond(e1, e2, e3), corresponding to this concrete syntax
> (known from C, C++, Java and C#):
> `e1 ? e2 : e3`

First step is to add `Cond(e1, e2, e3)` to the abstract syntax in `Absyn.fs`:
```fsharp
and expr =                                                         
  ...
  | Cond of expr * expr * expr       (* Conditional expression, changed 12.3 *)
```
For completeness sake we have also extended the parser and lexer with support for this. We won't go into detail about this, as its not part of the exercise.

To extend the compiler to compile conditional expressions to stack machine code, we do the following:
```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Cond(e1, e2, e3) ->
        let (jumpend, c1) = makeJump c // Create a jump that jumps to c
        let (labelse, c2) = addLabel ( cExpr e3 varEnv funEnv c1 ) // Add a label that evaluates the else case, i.e. e3
        cExpr e1 varEnv funEnv (addIFZERO labelse (cExpr e2 varEnv funEnv (addJump jumpend c2)))
```

> (ii) Check that your compiler compiles the following two examples to code that works properly: `true ? 1111 : 2222` and `false ? 1111 : 2222`

```fsharp
> open Absyn;;
> Comp.cExpr (Cond(CstI 1, CstI 1111, CstI 2222)) ([], 0) [] [];;
val it: Machine.instr list =
  [CSTI 1111; GOTO "L0"; Label "L1"; CSTI 2222; Label "L0"]
```
As we can see it already optimizes parts of it. It realises that `true ? 1111 : 2222` is trivially true and omits `CSTI 1111` with an instant jump to the end skipping the else case.

For the second example:
```fsharp
> Comp.cExpr (Cond(CstI 0, CstI 1111, CstI 2222)) ([], 0) [] [];;                                                                                                                                                                          
val it: Machine.instr list = [Label "L5"; CSTI 2222; Label "L4"]
```
Here we can observe similar behaviour but in this case we entirely skip the boolean check and are already at the else case.


# Exercise 12.4
> The compilation of the short-cut logical operators (&&) and (||) in Contcomp.fs is rather complicated. After Exercise 12.3 one can implement them
in a somewhat simpler way, using these equivalences:
> 
> `e1 && e2 is equivalent to (e1 ? e2 : 0)`
> `e1 || e2 is equivalent to (e1 ? 1 : e2)`
> 
> Implement the sequential logical operators (&& and ||) this way in your extended compiler from Exercise 12.3.
> You should change the parser specification in CPar.fsy to build Cond(...) expressions instead of Andalso(...) or
> Orelse(...). Test this approach on file ex13.c and possibly other examples. How does the code quality compare to the existing complicated compilation of &&
> and ||?

The first step is chasing the parser specification:
```
ExprNotAccess:
    AtExprNotAccess                     { $1                   }
  ...
  | Expr SEQAND Expr                    { Cond($1, $3, CstI 0) }
  | Expr SEQOR  Expr                    { Cond($1, CstI 1, $3) }
  | Expr QUESTION Expr COLON Expr       { Cond($1, $3, $5)     }
;
```

The old compiler using `AndAlso` and `AndElse` compiles to:
```fsharp
> compileToFile (fromFile "CEx/ex13.c") "CEx/ex13.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD; IFNZRO "L3"; GETBP; CSTI 1; ADD; LDI; CSTI 100; MOD;
   IFNZRO "L4"; GETBP; CSTI 1; ADD; LDI; CSTI 400; MOD; IFNZRO "L3";
   Label "L4"; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; Label "L3"; GETBP;
   CSTI 1; ADD; LDI; GETBP; LDI; LT; IFNZRO "L2"; RET 1]

> 
```
Using our new approach we get:
```fsharp
> compileToFile (fromFile "CEx/ex13.c") "CEx/ex13new.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD; IFNZRO "L5"; GETBP; CSTI 1; ADD; LDI; CSTI 100; MOD;
   IFZERO "L6"; CSTI 1; GOTO "L4"; Label "L6"; GETBP; CSTI 1; ADD; LDI;
   CSTI 400; MOD; NOT; GOTO "L4"; Label "L5"; CSTI 0; Label "L4"; IFZERO "L3";
   GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; Label "L3"; GETBP; CSTI 1; ADD;
   LDI; GETBP; LDI; LT; IFNZRO "L2"; RET 1]
```

The most obvious observation is that naturally the newer approach is simpler conceptually, but the amount of instructions we get are actually larger, so it is slower exxecuting than the old approach. We believe part of it is due to `addCST` not being able to constant propagation, because there is a label infront of `IFZERO`.