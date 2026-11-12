
# Exercise 13.1

> Download SML from the book homepage, unpack it and build micro-SML compiler as explained in the README.

This has been done.

> 1. What is the result value of running ex09.out?

The result value is 4.

```
dotnet run -eval SmlEx/ex09.sml
```

> 2. What type does the result value have? (Look at the rsult produced by the interpreter.)


By running the compiler with the `verbose` flag like so:

```
dotnet run -verbose SmlEx/ex09.sml
```

We see that it's an int.

```
VERBOSE: Result type: int
```

> 3. What application calls have been annotated as tail calls? Explain how this matches the intution behind a tail call. 

In this function we can see that in the function `f`, both the calls `g 4` and `f (x-1)` are in tail position.

```sml
fun f x = if x < 0 then g 4 else f (x-1)
and g x = x  

begin
  print (f 2)
end
```

We can see that there are tail calls when we compile with optmizations
i.e. `-opt` flag.


```fsharp
[ Label "G_ExnVar_L2";
    CSTI 0; CSTI 0; STI;

Label "G_Valdecs_L3";
    HEAPALLOC (1, 1); HEAPALLOC (1, 1);
    PUSHLAB "LabFunc_f_L4"; CSTI 1; LDI;
    HEAPCOPY 1; INCSP -1; PUSHLAB "LabFunc_g_L5";
    CSTI 2; LDI; HEAPCOPY 1; INCSP -1;
    GETSP; CSTI 2; SUB;
    CALL (0, "L1");
    STI; INCSP -3; STOP;

Label "LabFunc_f_L4";
    GETBP; CSTI 1; ADD; LDI; CSTI 0; LT;
    IFZERO "L6"; CSTI 2; LDI; 
    CSTI 4; // g 4
    TCLOSCALL 1; // TAIL CALL

Label "L6";
    GETBP; LDI; GETBP; CSTI 1;
    ADD; LDI; 
    CSTI 1; SUB; // f (x - 1)
    TCLOSCALL 1; // TAIL CALL

Label "LabFunc_g_L5";
    GETBP; CSTI 1; ADD; LDI; RET 2;

Label "L1";
    CSTI 1; LDI; CSTI 2; CLOSCALL 1; PRINTI;
    RET 0
]
```

> 4. What type has been annoated for the call sites to the functions `f` and `g`? Function `f` is called in two places, and `g` in one place.


At the first call site of `g`, we see that the function has the type
`int -> int` and then we call it with the value `4` which as type `int`.
The final type of the whole call expression is `int`.

At the first call site of `f`, we see that the function is also typed
`int -> int`, the argument is the expression `x:int - 1:int`. which has the final type `int`. This expression is then given to `f` as an argument and the final type of the whole call is `int`.

The second call site of `f`, the function is still typed `int -> int`.
The argument given is `2` which is typed `int`. `2` is applied to `f`
which produces the final type `int`.

```fsharp
VERBOSE: Program with types:
fun f:(int -> int) x = if (x:int < 0:int):bool then g:(int -> int) 4:int:int else f:(int -> int) (x:int - 1:int):int:int
and g:(int -> int) x = x:int
begin
  print(f:(int -> int) 2:int:int):int
end
```

> 5. What is the running time for executing the example using the evaluator, and what is the running time using the byte code `ex09.out` using `msmlmachine`?


On our test machine, it's 0 milli-seconds.

```
Result value: 4
Used 0 cpu milli-seconds
Number of GC: 0
```

> 6. Now compile the example `ex09.sml` without optimizations. How many byte code instructions did the optimization save for this small example?


Without optimizations it's 64 instructions.

With optimizations it's 59 instructions.

So we save 5 instructions. It's also worth mentioning that some calls are
turned into tail calls.

```fsharp
// num of ins: 64

[Label "G_ExnVar_L2"; CSTI 0; CSTI 0; STI; Label "G_Valdecs_L3";
 HEAPALLOC (1, 1); HEAPALLOC (1, 1); PUSHLAB "LabFunc_f_L4"; CSTI 1; LDI;
 HEAPCOPY 1; INCSP -1; PUSHLAB "LabFunc_g_L5"; CSTI 2; LDI; HEAPCOPY 1; INCSP -1;
 GETSP; CSTI 2; SUB; CALL (0, "L1"); STI; INCSP -3; STOP; Label "LabFunc_f_L4";
 GETBP; CSTI 1; ADD; LDI; CSTI 0; LT; IFZERO "L7"; CSTI 2; LDI; CSTI 4;
 CLOSCALL 1; GOTO "L6"; Label "L7"; GETBP; CSTI 0; ADD; LDI; GETBP; CSTI 1; ADD;
 LDI; CSTI 1; SUB; CLOSCALL 1; Label "L6"; RET 2; Label "LabFunc_g_L5"; GETBP;
 CSTI 1; ADD; LDI; RET 2; Label "L1"; CSTI 1; LDI; CSTI 2; CLOSCALL 1; PRINTI;
 RET 0]
```

With optimizations.

```fsharp
// num of ins: 59

[Label "G_ExnVar_L2"; CSTI 0; CSTI 0; STI; Label "G_Valdecs_L3";
 HEAPALLOC (1, 1); HEAPALLOC (1, 1); PUSHLAB "LabFunc_f_L4"; CSTI 1; LDI;
 HEAPCOPY 1; INCSP -1; PUSHLAB "LabFunc_g_L5"; CSTI 2; LDI; HEAPCOPY 1; INCSP -1;
 GETSP; CSTI 2; SUB; CALL (0, "L1"); STI; INCSP -3; STOP; Label "LabFunc_f_L4";
 GETBP; CSTI 1; ADD; LDI; CSTI 0; LT; IFZERO "L6"; CSTI 2; LDI; CSTI 4;
 TCLOSCALL 1; Label "L6"; GETBP; LDI; GETBP; CSTI 1; ADD; LDI; CSTI 1; SUB;
 TCLOSCALL 1; Label "LabFunc_g_L5"; GETBP; CSTI 1; ADD; LDI; RET 2; Label "L1";
 CSTI 1; LDI; CSTI 2; CLOSCALL 1; PRINTI; RET 0]
```

# Exercise 13.2

> In this exercise we extend micro-SML with a pair
 expression (e1, e2).
 > We can create a pair, and we access the first and second components of a pair `p` by `fst(p)` and `snd(p)`.

 > The example program `pair.sml` shows use of pairs where the pair's first and second components are accessed. 

Hello TA grading this `o/`. We have intentionally omitted some of the minor
code changes in an attempt to make reading it more bearable. Despite our best
efforts, this section remains painfully long.

Have fun!


> 1. Write type rules for the primitives `fst` and `snd`, see Fig. 13.6

Essentially we just say that if we want to show that `fst (l, r)` typechecks to `t`, then we must show that `l` is of type `t`.

For `snd (l, r)` we just check that `r` types to `t`.

```
      l : t                      r : t
---------------- fst_t      ---------------- snd_t
  fst (l, r) : t              snd (l, r) : t
```


> 2. Write evaluation rules for the primitives `fst` and `snd`, see Fig. 13.7

The idea is we can conclude that the function `fst` or
`snd` evaluates to the value `v` if we can show that the
corresponding element also evaluates to `v`.

```
    p |- l => v                   p |- r => v
--------------------- fst     ------------------ snd
p |- fst (l, r) => v           p |- snd (l, r) => v
```


> 3. `FunLex.fsl`: Extend with token `COMMA` and unary primitives `snd` and `fst`.

We've added support for a comma like so.

```fsharp
rule Token = parse
  ...
  | ','             { COMMA }
  ...
```

And we've added the unary functions as keywords.

```fsharp
let keyword s =
    match s with
    ..
    | "fst"       -> FST (* CHANGED | Exercise 13.2 *)
    | "snd"       -> SND
    ...
```

> 4. `FunPar.fsy`: Extend with the token `COMMA` and a rule for creating a pair. The concrete syntax is `(e1, e2)`


First we register the comma token.

```
%token COMMA /* CHANGED | 13.2 */
```

We make sure that `FST` and `SND` have the same precedence as `HEAD` and `TAIL`.

```
%nonassoc PRIM1 RAISE HEAD TAIL NOT FST SND          /* highest precedence  */
```

We need to make sure that all other binary operators bind tighter than the comma, i.e. so that

`(4 + 2, 2 * 3)` is parsed as:

```
    ,
  /   \
  +    *
4  2  2  3
```

So we put the precedence of `COMMA` between `IF` and `SEQOR`.

```
...
%nonassoc IF ELSE
%left COMMA        /* CHANGED | 13.2 */
%left SEQOR
...
```

Now we add the production rule for parsing a pair.

```fsharp
Expr:
    ...
  | Expr COMMA Expr           { Pair($1, $3, None)                   }
  ...
```

> 5. `Absyn.fs`: Extend the abstract syntax with a pair expression:

We simply add a pair type to our AST:

```fsharp
type expr<'a,'b> = 
  ...
  | Pair of expr<'a,'b> * expr<'a,'b> * 'a option // CHANGED | 13.2
  ...
```

Then we update all of the pair expression compiler functions that match on an `expr`.

```fsharp
let rec getOptExpr e : 'a Option =
  match e with
  ...
  | Pair(_, _, aOpt) -> aOpt
  ...
```

For tail calls, only the `snd` element is in tail position.

```fsharp
let tailcalls p : program<'a,'b> =
  let rec tc' tPos e =
    match e with
    ...
    | Pair(l, r, aOpt) -> Pair(tc' false l, tc' true r, aOpt)
    ...
```

For free vars, we simply append the sets of free variables for each expression.

```fsharp
let rec freevars e : string Set =
  match e with 
  ...
  | Pair (l, r, _) -> freevars l + freevars r (* CHANGED | 13.2 *)
```

> 6. `TypeInference.fs`: Extend the `typ` type with the new pair type: 

We simply add our type to `typ`.

```fsharp
type typ =
  ...
  | TypP of typ * typ                   // Pair CHANGED | 13.2
  ...
```


Then we extend our function for resolving types to take pairs into account.

```fsharp
let rec resolveType t0 = 
  match t0 with
  ...
  | TypP(l, r) -> TypP (resolveType l, resolveType r)
  ...
```

Then we extend `freeTypeVars` to take care of pairs.

```fsharp
let rec freeTypeVars t : typevar list = 
  match normType t with
  ...
  | TypP(t1, t2) -> union(freeTypeVars t1, freeTypeVars t2)
```

We also extend `typeToString` so that the pair type can be pretty printed. We figure out the types `t1` and `t2` recursively.

```fsharp
let rec typeToString t : string =
  match t with
  ...
  | TypP(t1, t2) -> "pair(" + typeToString t1 + ", " + typeToString t2 + ")"
```

For the unify types function, we check that two pairs can be
unified by checking that their inner types match. That is the type
of the first element in the first pair can be unified with the
first element in the second pair. And that the type of the second
element in the first pair can be unified with the type of the
second element in the second pair.

```fsharp
let rec unify t1 t2 : unit =
  CmdLine.debug ("Unifying type " + (showType t1) + " with type " + (showType t2) + ".")
  let t1' = normType t1
  let t2' = normType t2
  match (t1', t2') with
  | TypP(t1, t2), TypP(t3, t4) -> unify t1 t3
                                  unify t2 t4
  ...
```

For the `copyType` function, we need to remember to recursively copy
the types in the pair.

```fsharp
let rec copyType subst t : typ = 
  match t with
  ...
  | TypP(t1,t2) -> TypP(copyType subst t1, copyType subst t2)
```

The `showType` function is handled exactly as the `typeToString` function.

```fsharp
let rec showType t : string =
  let rec pr t = 
    match normType t with
    ...
    | TypP(t1, t2) -> "pair(" + pr t1 + ", " + pr t2 + ")" (* CHANGED | 13.2 *)
```

The `typExpr` function sucked the souls out of us.

It made us envy the lives of maggots.

But after much effort, we made something that works.

It's not possible for us to explain how this works. Just stare at
it until you either understand it or experience the same feeling of
absolute despair this single function put us through.

```fsharp
let rec typExpr (lvl : int) (env : tenv) (e : expr<'a,'b>) : typ * expr<typ,typescheme> =
  match e with
  | Pair(l, r, _) -> let lt, _ = typExpr lvl env l
                     let rt, _ = typExpr lvl env r
                     let ts = TypP(lt, rt)
                     ts, Pair(l, r, Some ts)
  ...
  | Prim1(ope,e1,_) -> 
    let (t1,e1') = typExpr lvl env e1
    match ope with
    ...
    | "fst" -> let tv = TypV(newTypeVar lvl)
               tv,Prim1(ope,e1',Some tv)

    | "snd" -> let tv = TypV(newTypeVar lvl)
               tv,Prim1(ope,e1',Some tv)

    ...
  ...
```

> 7. `HigherFun.fs`: The interpreter must be able to handle pair values.

We simply add a constructor for the pair type.

```fsharp
type value = 
  ...
  | PairV of value * value
  ...
```

We then extend `ppValue` so that it also supports pretty printing pairs. We simply recursively print out the value of the pair.


```fsharp
let rec ppValue = function
  ...
  | PairV(l, r) -> sprintf "( %s , %s )" (ppValue l) (ppValue r)
```

Then for the `evalExpr` function we simply add a case to evaluate a pair into a
value. We also define the behaviour of the `fst` and `snd` functions.

```fsharp
let rec evalExpr (env : value env) (e : expr<typ,typescheme>)
                 (cont: value -> answer) (econt: value -> answer)  : answer =
  match e with
  | Pair(l, r, _) -> evalExpr env l (fun l' -> 
                          evalExpr env r (fun r' ->
                                  cont( PairV(l', r') )
                             ) econt) econt
  ...
  | Prim1(ope,e1,_) ->
    evalExpr env e1 (fun v1 ->
                     match (ope,getTypExpr e1) with
                     ...
                     | ("fst",_) -> match v1 with
                                      | PairV(l,_) -> cont l
                                      | _ -> Abort ("fst: on non-pair value " + (ppValue v1))
                     | ("snd",_) -> match v1 with
                                      | PairV(_,r) -> cont r
                                      | _ -> Abort ("snd: on non-pair value " + (ppValue v1))
                    ...
  ...
```

Finally we add support for the `check` function. We simply recursively call
check on both elements in the pair.

```fsharp
let check e =
  let rec check' (e : expr<'a,'b>) (env : string list) : bool =
    match e with
    ...
    | Pair (l,r,_) -> check' l env && check' r env
```

> 8. `msmlmachine.c`: Two new byte code instructions are needed. One for creating a pair, `PAIR` and one for printing a pair, `PRINTP`. To simplify matters you can implement `PRINTP` assuming that pairs will always contain scalar values. This is of course not always the case.


```
42 PAIR       s, v1, v2  => s, p         Create pair cell (v1, v2) on heap
44 PRINTP     s, p       => s, p         Print pair value pointer at by p
```

Note that the specified instruction numbers `42` and `44` are already in use by instructions `HEAPSTI` and `HEAPALLOC`.

We've therefore arbitrarily decided to use `115` as our instruction number for
`PAIR` and `116` has the instruction number for `PRINTP`.

Firstly the code for handling a `PAIR` is almost identical with the code for
`CONS`. The only difference is the tag used in the block.

We allocate a 2 word block and set the tag to a `PAIRTAG`. Then we pop off the
first two elements from the stack and add them to the pair.

For the `PRINTP` instruction, we've defined a new helper function called
`printPair`, see the code block below this one.

```c
int execcode(word p[], word s[], word iargs[], int iargc, int trace) {
  word bp = -999;
  word sp = -1;          // Stack top pointer
  word pc = 0;           // Program counter: next instruction
  word hr = -1;          // Handler Register
  for (;;) {
    ...
    switch (p[pc++]) {
    ...
    case PAIR: {
      word* p = allocate(PAIRTAG, 2, s, sp); 
      p[1] = (word)s[sp-1];
      p[2] = (word)s[sp];
      s[sp-1] = (word)p;
      sp--;
    } break;
    ...
    case PRINTP:
      printPair(s[sp]); break;
    ...
    }
  }
  ...
}
```

This is the code for handling printing pairs. We essentially just call the
`printVal` function for each element in the pair.

```c
// CHANGED | 13.2
void printPair(word i) {
    if (i == NILVALUE) {
        printNil(i);
        return;
    } 
    word *pairPtr = (word *)i;

    if (BlockTag(*pairPtr) != PAIRTAG) {
      printf("PRINTP: Expected PAIRTAG.\n");
      exit(-1);
    }

    printf("( ");
    printVal(pairPtr[1]);
    printf(", ");
    printVal(pairPtr[2]);
    printf(") ");
}
```

We've then modified `printVal` so that it's aware of the new `PAIRTAG`. This
allows for the printing of nested pairs.

```c
void printVal(word i) {
  // print all values - scalar as integers, pointers using tag
  if IsInt(i) {
    printI(i);  // A scalar value printed as integer
    return;
  }
  if (i == NILVALUE) {
    printNil(i);  // A pointer null value.
    return;
  }
  // Reference, look at tag
  word *valPtr = (word *)i;

  ... // Ommitted for the sake of brevity.

  // CHANGED | 13.2
  if (BlockTag(*valPtr) == PAIRTAG) {
    printPair(i);
    return;
  } 
}
```



> 9. `ContComp.fs`: The compiler must be extended to generate code for creating a
pair in the heap and for accessing the first and second component with fst and
snd respectively.

We make a `Pair` compile to the `PAIR` instruction mentioned earlier.

Since the layout of a pair is identical with that of a `CONS`, we can re-use
the `CAR` and `CDR` instructions. `fst` is compiled to `CAR` and `snd` is
compiled to `CDR`.


```fsharp
let rec cExpr (kind: int->var) (varEnv : varEnv) (e : expr<typ,typescheme>) (C: instr list) : instr list =
  let (env,fdepth) = varEnv
  match e with
  ...
  | Pair(l, r, _) -> cExpr kind varEnv l (cExpr kind varEnv r (PAIR :: C))
  ...
  | Prim1(ope,e1,_) ->
    cExpr kind varEnv e1 
      (match (ope,getTypExpr e1) with
       ...
       | ("fst",_)    ->  CAR :: C  
       | ("snd",_)    ->  CDR :: C
       ...
  | Prim2(ope, e1, e2,_) -> ...
  ...
```

> 10. `Machine.fs`: The byte code instructions PRINTP and PAIR must be added. You
have to assign unique instruction codes to PRINTP and PAIR that match with
same instructions in msmlmachine.c.

First we define the two new instructions.

```fsharp
type instr =
  ...
  | PRINTVAL                  // Polymorphic print
  ...
  | PAIR

  ...
```

Then we assign each instruction a byte code.

```fsharp
let code = function
  ...
  | PAIR  -> 115
  | PRINTP  -> 116
```

We specify that both of these instructions are 1 byte instructions.

```fsharp
let sizeInstr instr = 
  match instr with
  ...
  | PRINTP         -> 1
  ...
  | PAIR           -> 1
```

Then we add support for pretty printing them.

```fsharp
let ppInstr (addr,strs) instr =
  let indent s = (addr + sizeInstr instr,"  " + (addr.ToString().PadLeft(4)) + ": " + s :: strs)
  match instr with
  ...
  | PRINTP         -> indent "PRINTP" (* CHANGED | 13.2 *)
  ...
  | PAIR           -> indent "PAIR"
  ...
```

## And now we test!

Finally we can test that the example presented in the book works. Looking at
the example below, `f` should call itself recursively twice and then it should
call `g`, which should produce the output `(-1, 42)`.

```sml
val p = (1,43)
fun f p = if fst(p) < 0 then g p else f (fst(p)-1, snd(p))
and g p = (fst(p),snd(p)-1)

begin
  print (f p)
end
```

First we test that the interpreter works by passing the `eval` flag to the compiler.
```
dotnet run -eval SmlEx/pair.sml
```

And we get the following output as expected.
```
Evaluating Program
( -1, 42 )
Result value: (-1, 42)
Used: Elapsed 16ms, CPU 16ms
 
```

Then we run the compiled program using the VM and observe that we get the same result.

```
../MicroVM/microvm SmlEx/pair.out
( -1 , 42 )
Result value: #128980830982264
Used 0 cpu milli-seconds
Number of GC: 0
```

The only minor difference is that the result value is the pointer for the pair.
This is due to how the `printStackVal` function is implemented in the VM.

