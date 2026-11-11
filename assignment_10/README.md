
# Exercise 12.1

> Improve the compiler so that it optimizes `IFZERO L3; GOTO L2; Label L3; ...`

> to `IFNZRO L2; Label L3; ...`

To start off, we first write a MicroC program which generates this bad bytecode.

We have done so in `sanity/if_sanity.c`.
```c
void main(int n) {
    int isPos;
    isPos = 0;

    if (n < 0) 
    {  }
    else {
        isPos = 1;
    }

    print isPos;
    println;
}
```

Now we want to run it and see that this ineeds produces the bad bytecode.

First we compile the compiler and start it up in an interactive environment.

```
dotnet build microcc.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll \
    Absyn.fs CPar.fs CLex.fs Parse.fs \
    Machine.fs Contcomp.fs ParseAndComp.fs
```

Then we compile the example file. We have formatted the emitted bytecode so that it's easier to see the bad byte code.

```fsharp
open ParseAndComp;;
> compileToFile (fromFile "sanity/if_sanity.c") "sanity/if_sanity.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 0; STI; INCSP -1; GETBP; LDI; CSTI 0; LT;

   IFZERO "L3"; GOTO "L2"; Label "L3"; GETBP; CSTI 1; ADD; CSTI 1;

   STI; INCSP -1; Label "L2"; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; CSTI 10; PRINTNL; RET 2]
```

Now we have some code that we can actually use to test if our optimization
works.

So, first we define a helper function that performs the optimizaiton in
`Contcomp.fs`.

The idea is whatever is in our continuation right now is the code our `IFZERO` is placed in front of. We simply check that if the next instruction is a `GOTO lab2`, in which case we perform the optimization.

If it isn't possible to perform the optimization, then this helper function
will just compile the the instruction normally.

```fsharp
(* CHANGED | 12.1 *)
let addIFZERO lab3 c =
    match c with
    | (GOTO lab2 :: c1) -> IFNZRO lab2 :: c1
    | _                 -> IFZERO lab3 :: c
```

Now we modify the function for compiling statements `cStmt` so that it uses our optimizing helper function.

```fsharp
let rec cStmt stmt (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list = 
    match stmt with
    | If(e, stmt1, stmt2) -> 
      let (jumpend, c1) = makeJump c
      let (labelse, c2) = addLabel (cStmt stmt2 varEnv funEnv c1)

      (* CHANGED | 12.1 *)
      let ifcomp = addIFZERO labelse (cStmt stmt1 varEnv funEnv (addJump jumpend c2))
      cExpr e varEnv funEnv ifcomp
    | While(e, body) ->
    ...
```

There are also several places in the `cExpr` function where we might be able to apply this optimization.

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Andalso(e1, e2) ->
      match c with
      | IFZERO lab :: _ ->
         cExpr e1 varEnv funEnv (addIFZERO lab (cExpr e2 varEnv funEnv c))
      | IFNZRO labthen :: c1 -> 
        let (labelse, c2) = addLabel c1
        cExpr e1 varEnv funEnv
           (addIFZERO labelse
               (cExpr e2 varEnv funEnv (addIFNZRO labthen c2)))
      | _ ->
        let (jumpend,  c1) = makeJump c
        let (labfalse, c2) = addLabel (addCST 0 c1)
        cExpr e1 varEnv funEnv
          (addIFZERO labfalse
             (cExpr e2 varEnv funEnv (addJump jumpend c2)))
    | Orelse(e1, e2) -> 
      match c with
      | IFZERO labthen :: c1 ->
        let(labelse, c2) = addLabel c1
        cExpr e1 varEnv funEnv
           (IFNZRO labelse :: cExpr e2 varEnv funEnv
             (addIFZERO labthen c2))
    ...
```

Finally we recompile the same test from earlier `sanity/if_sanity.c` and see
that the optimization works. Again, we've reformatted the output of the
compilation so that's it's easier to read.

```fsharp
open ParseAndComp;;
> compileToFile (fromFile "sanity/if_sanity.c") "sanity/if_sanity.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 0; STI; INCSP -1; GETBP; LDI; CSTI 0; LT;

   IFNZRO "L2"; Label "L3"; GETBP; CSTI 1; ADD; CSTI 1; STI; INCSP -1; Label "L2"; 
   GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; CSTI 10; PRINTNL; RET 2]
```


The exercise also mentions:

> A similar optimization can be made for IFNZRO L3; GOTO L2; Label L3. This is done in much the same way. 

Due to the fact that the approach is essentially identical to before, we've
omitted some of the steps due to brevity. 

We first make the helper method like so:

```fsharp
(* CHANGED | 12.1 *)
let addIFNZRO lab3 c =
    match c with
    | (GOTO lab2 :: c1) -> IFZERO lab2 :: c1
    | _                 -> IFNZRO lab3 :: c

```

And than we substitute all manual constructions of `IFNZERO` with `addIFNZRO`.


# Exercise 12.2

> Improve code generation in the cont-based micro-C compiler so that a less-than comparison with constant arguments is compiled to its truth value.

First we write a test case in `sanity/cmp_sanity.c` which generates the inefficient byte code.

```c
int main() {
    if(11 < 22) {
        print 1;
    }

    if(22 < 11) {
        print 2;
    }

    println;
}
```


Compiling this we get(adjustments made to output so that it's easier to see the bad bytecode):
```fsharp
> compileToFile (fromFile "sanity/cmp_sanity.c") "sanity/cmp_sanity.out";;
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; 
  CSTI 11; CSTI 22; LT;
   IFZERO "L3"; CSTI 1; PRINTI; INCSP -1; Label "L3";
   CSTI 22; CSTI 11; LT;
   IFZERO "L2"; CSTI 2; PRINTI; INCSP -1; Label "L2"; CSTI 10; PRINTNL; RET 0]
```

The way we fix this is by simply updating `addCST` to check for this case.

```fsharp
let rec addCST i c =
    match (i, c) with
    ...
    | (_, CSTI j :: LT :: c1) -> if i < j
                                 then addCST 1 c1
                                 else addCST 0 c1
    | _                     -> CSTI i :: c
```

Now we recompile and see that our change worked. Again we've reformatted the
output so that's it's easier to see. But this time around we can see that
we're simply pushing a single true or false constant on the stack rather than
performing a comparison.

```fsharp
> compileToFile (fromFile "sanity/cmp_sanity.c") "sanity/cmp_sanity.out";;
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; 
  CSTI 1; IFZERO "L3"; CSTI 1; PRINTI; INCSP -1; Label "L3"; 
  CSTI 0; IFZERO "L2"; CSTI 2; PRINTI; INCSP -1;
   Label "L2"; CSTI 10; PRINTNL; RET 0]
```

> Further improve the code generation so that all comparisons with constant arguments are compiled to the same code as true (e.g. 11 <= 22 and 11 != 22 and 22 > 11 and 22 >= 11) or false.


First we just need to realize that all other operators are expressed in terms of `LT` and `NOT`.

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Prim2(ope, e1, e2) ->
      cExpr e1 varEnv funEnv
        (cExpr e2 varEnv funEnv
           (match ope with
            ...
            | "<"   -> LT   :: c
            | ">="  -> LT   :: addNOT c
            | ">"   -> SWAP :: LT :: c
            | "<="  -> SWAP :: LT :: addNOT c
            ...
    ...
```


The way we've decided to solve this to add a helper function `addPRIM2` which
we use when compiling the value for `e1`. This can check for all of the
comparisons cases. It also allows for future optimizations to the arithmetic
operations.

```fsharp
let rec addPRIM2 c =
    match c with
    | (CSTI i :: CSTI j :: EQ :: c1) -> if i = j
                                        then addPRIM2 (CSTI 1 :: c1)
                                        else addPRIM2 (CSTI 0 :: c1)
    | (CSTI i :: NOT :: c1) -> if i = 0
                               then addPRIM2 (addCST 1 c1)
                               else addPRIM2 (addCST 0 c1)
    | (CSTI i :: CSTI j :: SWAP :: c1) -> addPRIM2 (addCST j (addCST i c1))
    | (CSTI i :: CSTI j :: LT :: c1) -> if i < j
                                        then addPRIM2 (addCST 1 c1)
                                        else addPRIM2 (addCST 0 c1)

    | (CSTI 0 :: IFZERO lab :: c1) -> GOTO lab :: c1
    | (CSTI _ :: IFZERO lab :: c1) -> c1
    | (CSTI n :: IFNZRO lab :: c1) when n <> 0 -> GOTO lab :: c1
    | (CSTI _ :: IFNZRO lab :: c1) -> c1
    | _ -> c
```

Now we update the the `cExpr` function to use `addPRIM2` when compiling
`Prim2`.

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Prim2(ope, e1, e2) ->
      addPRIM2 (cExpr e1 varEnv funEnv)
        (cExpr e2 varEnv funEnv
           (match ope with
            ...
            ))
    ...
```

Now we create a test case.

```c
int main() {
    if(11 < 22) {
        print 1;
    }

    if(22 < 11) {
        print 2;
    }

    if(22 >= 11) {
        print 3;
    }

    if(11 >= 22) {
        print 4;
    }

    if(22 <= 11) {
        print 5;
    }

    if(11 <= 22) {
        print 6;
    }

    println;
}
```

And compile it. As we can see, everything has been nicely optimized.

```fsharp
> compileToFile (fromFile "sanity/cmp_sanity_2.c") "sanity/cmp_sanity_2.c";;
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP;
  Label "L1"; CSTI 1; PRINTI; INCSP -1;
  Label "L4"; CSTI 3; PRINTI; INCSP -1;
  Label "L3"; CSTI 6; PRINTI; INCSP -1;
  Label "L2"; CSTI 10; PRINTNL; RET 0]
```

# Exercise 12.3

> Extend the micro-C abstract syntax (file `Absyn.fs`) with conditional expressions `Cond(e1, e2, e3)`, corresponding to this concrete syntax(known from C, C++, Java and C#): `e1 ? e2 : e3`

While the exercise specifically mentions that parsing this syntax isn't specifically required, for the sake of completeness, we've done so.

First we define the abstract syntax for a ternary.

```fsharp
type typ =
...
and expr =                                                         
...
| Ternary of expr * expr * expr    (* Ternary operator CHANGED | 12.3 *)
```

Then we define two new tokens in the lexer.

```fsharp
rule Token = parse
  ...
  | '?'             { QUESTION }  (* CHANGED | 12.3 *)
  | ':'             { COLON } 
  ...
```

Then we register the tokens within the parser.

```
%token COLON QUESTION  /* CHANGED 12.3 */
```

And correctly ensure that they don't bind tigther than the binary arithmetic
operators.

```
%right ASSIGN             /* lowest precedence */
%nonassoc PRINT
%left QUESTION COLON /* CHANGED | 8.5 */
%left SEQOR
...
%nonassoc LBRACK          /* highest precedence  */
```

Then we add a rule that correctly parses ternary expressions. 

```fsharp
ExprNotAccess:
    AtExprNotAccess                     { $1                   }
  | Expr QUESTION Expr COLON Expr       { Ternary($1, $3, $5)  } /* CHANGED | 12.3 */
  ...
```

Finally we add support for ternary operators in the compiler.

The idea is to achieve something akin to the following pseudo-bytecode:

```
<condition>
ifnzro lfalse
<e1>
GOTO end
LABEL lfalse
<e2>
LABEL end
```

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Ternary(condition, e1, e2) ->
      let (lend, c1) = addLabel c
      let (lcond, c2) = (addLabel (cExpr e1 varEnv funEnv c1))

      cExpr condition varEnv funEnv (
          addIFNZRO lcond (cExpr e2 varEnv funEnv (GOTO lend :: c2)))
```

To make sure this works, we've made a test case in `sanity/ternary_sanity.c`.

```c
// expected output: "2 1"
int main() {
    print (2 > 10) ? (0 + 1) : (2 * 2 - 2);
    print (10 > 2) ? (0 + 1) : (2 * 2 - 2);
    println;
}
```

Compiling it and running it we get:

```
$ java Machine.java sanity/ternary_sanity.out
2 1

Used 0.002 seconds
```

# Exercise 12.4

> The compilation of the short-cut logical operators `(&&)` and `(||)` in `Contcomp.fs` is rather complicated. After Exercise 12.3 one can implement them in a somewhat simpler way, using these equivalences:

> `e1 && e2` is equivalent to `(e1 ? e2 : 0)`

> `e1 || e2` is equivalent to `(e1 ? 1 : e2)`


> Implement the sequential logical operators (`&&` and `||`) this way in your extended compiler from Exercise 12.3

This is rather straight forward, we just replace the implementation of the
existing for compiling `Andalso` and `Orelse` with a ternary that gets
compiled.

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    match e with
    ...
    | Andalso(e1, e2) -> cExpr (Ternary(e1, e2, CstI 0)) varEnv funEnv c
    | Orelse(e1, e2) -> cExpr (Ternary(e1, CstI 1, e2)) varEnv funEnv c
```

> You should change the parser specification in `CPar.fsy` to build `Cond(...)` instead of `Andalso(...)`

This is also rather straightforward. Isntead of compiling to an `Andalso` or `Orelse`, we just use a `Ternary`.

```fsharp
ExprNotAccess:
  ...
  | Expr SEQAND Expr                    { Ternary($1, $3, CstI 0) }
  | Expr SEQOR  Expr                    { Ternary($1, CstI 1, $3) }
  ...
```

> Test this approach on file ex13.c and possibly other examples. How does the code quality compare to the existing complicated compilation of `&&` and `||`?


The following is compiled with `Ternary` being used for logical ors and logical ands.

There are 2 `IFNZRO` instructions in the `Ternary` version and 4 in the
original. However, there are 4 `GOTO` instructions in the `Ternary` as opposed
to 1 in the original.

Furthermore there are 69 instructions in the `Ternary` version vs. the 62 in the original.

The ternary version is worse here.


```fsharp
[  LDARGS 1; CALL (1, "L1"); STOP;

   Label "L1"; 
   INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1;
   GOTO "L3";

   Label "L2";
   GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI;
   INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD;
   IFZERO "L6";
   CSTI 0;
   GOTO "L4";

   Label "L6";
   GETBP; CSTI 1; ADD;
   LDI; CSTI 100; MOD;
   IFNZRO "L5";
   GETBP;
   CSTI 1; ADD; LDI; CSTI 400; MOD; NOT;
   GOTO "L4";

   Label "L5";
   CSTI 1;

   Label "L4";
   IFZERO "L3"; GETBP; CSTI 1;
   ADD; LDI; PRINTI; INCSP -1;

   Label "L3";
   GETBP; CSTI 1; ADD; LDI; GETBP; LDI;
   LT;
   IFNZRO "L2";
   RET 1
]
```

This is the original compiler.

```fsharp
  [LDARGS 1; CALL (1, "L1"); STOP; 

  Label "L1";
  INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1;
   GOTO "L3";

   Label "L2";
   GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI;
   INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD;
   IFNZRO "L3";
   GETBP; CSTI 1; ADD; LDI; CSTI 100; MOD;
   IFNZRO "L4";
   GETBP; CSTI 1; ADD; LDI; CSTI 400; MOD;
   IFNZRO "L3";

   Label "L4"; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1;

   Label "L3"; GETBP;
   CSTI 1; ADD; LDI; GETBP;
   LDI; LT;
   IFNZRO "L2";
   RET 1
   ]
```

