
# Exercise 7.4

> Extend the Micro-C abstract syntax in `Absyn.fs` with the preincrement and predecrement operatirs known from C, C++, Java nad C#


First we modify `Absyn.fs` and add the `PreInc` and `PreDec` constructors to the `expr` type.

```fsharp
type typ =
  ...
and expr =                                                         
  ...
  | PreInc of access                 (* CHANGED | 7.4 *)
  | PreDec of access                 (* CHANGED | 7.4 *)
```

Afterwards we simply add two cases to the `eval` function. One case is for the preincrement and the other is for the predecrement.

```fsharp
eval e locEnv gloEnv store : int * store = 
  match e with
    ...
    (* CHANGED | 7.4 *)
    | PreInc acc     -> let (loc, store1) = access acc locEnv gloEnv store
                        let new_int = (getSto store1 loc) + 1
                        (new_int, setSto store1 loc new_int)
    (* CHANGED | 7.4 *)
    | PreDec acc     -> let (loc, store1) = access acc locEnv gloEnv store
                        let new_int = (getSto store1 loc) - 1
                        (new_int, setSto store1 loc new_int)
    ...
```

# Exercise 7.5

> Extend the micro-C lexer and parser to accept ++e and --e also, and to build the corresponding abstract syntax.


We don't have any new tokens, so we don't need to modify `CPar.fsl`.

The only change needed here is in `CPar.fsy`, we simply add the two cases and make sure they correctly produce `PreInc` and `PreDec`.

```fsharp
ExprNotAccess:
...
| PLUS PLUS Access                    { PreInc($3) } /* CHANGED | 7.5 */
| MINUS MINUS Access                  { PreDec($3) }   
;
```

As a sanity check, we've made a simple test case in `sanity/inc_dec.c`.

```c
void main() {
    int x;
    x = 0;
    print ++x; // Should print 1
    print ++x; // Should print 2
    print --x; // Should print 1
    print ++x; // Should print 2
    print --x; // Should print 1
    print --x; // Should print 0
    print --x; // Should print -1
}
```

We now rebuild the project to re-generate our parser and run the sanity check.

```
dotnet build parse.fsproj

dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll \
    Absyn.fs CPar.fs CLex.fs Parse.fs \
    Machine.fs Comp.fs ParseAndComp.fs
```

Running our sanity check, we see the numbers printed out align with our expectations.
```fsharp
> open ParseAndRun;;
> run (fromFile "sanity/inc_dec.c") [];;
1 2 1 2 1 0 -1 val it: Interp.store = map [(0, -1)]
```

# Exercise 8.1

> Download microc.zip from the homepage, unpack it to a folder MicrOC, and build the micor-C compiler as explained in README.TXT step (B).

This has been done.

## Exercise 8.1.i

> As a warm-up, compile one of the micro-C examples provided, such as that in soruce file `ex11.c`, then run it using the abstract machine implemented in Java, as described also in step (B) of the README file. When run with the command line argument 8, the program prints the 92 solutions to the eight queens problem

We compiled `ex11.c` to bytecode, the output can be seen at `CEx/ex11.out`.

Running the byte code using the abstract machine with `8` as an argument yielded:

```bash
# Truncated output(due to it being very long)
$ java Machine.java CEx/ex11.out 8

1 5 8 6 3 7 2 4 
1 6 8 3 7 4 2 5 
1 7 4 6 8 2 5 3 
...
8 2 5 3 1 7 4 6 
8 3 1 6 2 5 7 4 
8 4 1 3 6 2 7 5 

Used 0.03 seconds
```

## Exercise 8.1.ii

> Now compile the example micro_C programs `ex3.c` and `ex5.c` using functions `compileToFile` and `fromFile` from `ParseAndCompl.fs` as above.
> Study the generated symbolic bytecode. Write up the bytecode in a more structured way with labels only at the beginning of the line (as in this chapter). Write the corresponding micro-C code to the right of the stack machine code.
> Note that `ex5.c` has a nested scope (a block ... inside a function body); how is that visible in the generated code?

### ex3.c breakdown

The bytecode file can be found at `CEx/ex3.out`.

```bash
LDARGS 1; # loads the single argument for the main function, adds n to stack
CALL (1, "L1"); # calls main function
STOP;
Label "L1"; # main function label

INCSP 1; GETBP; # Grow stack and put base pointer on top

CSTI 1;
ADD;
CSTI 0;
STI;
INCSP -1;
GOTO "L3";
Label "L2";
GETBP;
CSTI 1;
ADD;
LDI;
PRINTI;
INCSP -1;
GETBP;
CSTI 1;
ADD;
GETBP;
CSTI 1;
ADD;
LDI;
CSTI 1;
ADD;
STI;
INCSP -1;
INCSP 0;
Label "L3";
GETBP;
CSTI 1;
ADD;
LDI;
GETBP;
CSTI 0;
ADD;
LDI;
LT;
IFNZRO "L2";
INCSP -1;
RET 0

```


### ex5.c breakdown

> Execute the compiled programs using `java Machine ex3.out 10` and similar. Note that these micro-C programs require a command line argument (an integer) when they are executed.

ANSWER


> Trace the execution using `java Machinetrace ex3.out 4`, and explain the stack contents and what goes on in each step of execution, especially how the low-level bytecode instructions map to the higher-level features of MicroC.

ANSWER

# Exercise 8.3

> The abstract syntax fore preincrement `++e` and predecrement `--e` was introduced in Exercise 7.4.

> Modify the compiler (function `cExpr`) to generate code for `PreInc(acc)` and `PreDec(acc)`.

There's only one place where we need to change something and that's in `cExpr`, the function for compiling expressions.

```fsharp
...
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
...
| PreInc acc     -> cAccess acc varEnv funEnv @ [DUP; LDI; CSTI 1; ADD; STI] 
| PreDec acc     -> cAccess acc varEnv funEnv @ [DUP; LDI; CSTI 1; SUB; STI] 
...
```

To make sure this is correct, we compile the sanity check program from earlier(exercise 7.5).

```fsharp
> open ParseAndComp;;
> compileToFile (fromFile "sanity/inc_dec.c") "sanity/inc_dec.out";;
```

And then run the emitted bytecode inside the abstract machine.

```bash
$ java Machine.java sanity/inc_dec.out
1 2 1 2 1 0 -1
Used 0.001 seconds
```

The exercise also mentions to experiment with `++arr[++i]` if you're brave. Not sure about the being brave part, but we're dumb enough to test our luck. Let's see if we did things right.


Inside `sanity/brave_inc.c` you'll see the following microC code.
```c
void main() {
    int i;
    int arr[3];
    arr[0] = 10;
    arr[1] = 20;
    arr[2] = 30;
    i = 0;

    ++arr[++i];

    // expected array contents: 10 21 30 
    // expected i: 1
    print arr[0];
    print arr[1];
    print arr[2];
    println;
    print i;
}
```

Compiling this and running it we get:
```bash
$ java Machine.java sanity/brave_inc.out
10 21 30
1
Used 0.001 seconds
```

As expected! :)

# Exercise 8.4

> Compile `ex8.c` and study the symbolic bytecode to see why it is so much slower than the handwritten 20 million iterations loop in prog1.

> Compile ex13.c and study the symbolic bytecode to see how loops and conditionals interact; describe what you see.
