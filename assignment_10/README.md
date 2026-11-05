
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

And than we substitute all manual constructiosn of `IFNZERO` with `addIFNZRO`.


# Exercise 12.2

> Impr

First we write a test case in `sanity/cmp_sanity.c` which generates the inefficenet byte code.

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
                             then CstI 1 :: c1
                             else CstI 0 :: c1
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
let rec addPRIM2 e1 c =
    match (e1, c) with
    | (CSTI i, CSTI j :: EQ :: c1) -> if i = j
                                      then addPRIM2 (CSTI 1) c1
                                      else addPRIM2 (CSTI 0) c1
    | (CSTI i, NOT :: c1) -> if i = 0
                             then addPRIM2 (CSTI 1) c1
                             else addPRIM2 (CSTI 0) c1
    | (CSTI i, CSTI j :: SWAP :: c1) -> addPRIM2 (CSTI j) (CSTI i :: c1)
    | (CSTI i, CSTI j :: LT :: c1) -> if i < j
                                      then addPRIM2 (CSTI 1) c1
                                      else addPRIM2 (CSTI 0) c1
    | _ -> e1 :: c
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


