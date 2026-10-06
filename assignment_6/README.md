
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

**NOTE:** Please note that the bytecode for exercise 3 was generated using the `Contcomp.fs` compiler rather than the `Comp.fs` compiler by mistake. We did not want to rewrite the entirety of the exercise to change this, but now you know.

When executing `compileToFile (fromFile "CEx/ex03.c") "CEx/ex3.out";;` we get the following symbolic bytecode:
```fsharp
> compileToFile (fromFile "CEx/ex03.c") "CEx/ex3.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   GETBP; LDI; STI; GETBP; LDI; GETBP; CSTI 2; ADD; CALL (2, "L2"); INCSP -1;
   GETBP; CSTI 2; ADD; LDI; PRINTI; INCSP -2; GETBP; CSTI 1; ADD; LDI; PRINTI;
   RET 2; Label "L2"; GETBP; CSTI 1; ADD; LDI; GETBP; LDI; GETBP; LDI; MUL;
   STI; RET 2]
```
This code can be written in a more structured way with the corresponding MicroC code to the right:

```fs
   LDARGS 1
   CALL 1, L1                // calls main with its argument
   STOP
L1:
   INCSP 1                   // void main(int n) {...};
   GETBP; CSTI 1 ADD;        //         note: computes the stack address of i
   CSTI 0; STI;              // i = 0;
   INCSP -1                  //         note: STI stores the assignment value on the stack, this drops it
   GOTO "L3"                 //         note: L3 is the guard of the while loop
L2:                          //         note: L2 corresponds to the body of the while loop     
   GETBP; CSTI 1; ADD;       //         note: compute stack address of i
   LDI                       //         note: load value of i
   PRINTI                    // print i; 
   INCSP -1                  //         note: remove value of i from the stack
   GETBP; CSTI 1; ADD        //         note: compute stack address of i
   GETBP; CSTI 1; ADD        //         note: compute stack address of i
   LDI                       //         note: load value of i
   CSTI 1; ADD; STI;         // i = i + 1;
   INCSP -1                  //         note: remove stored value from stack
L3:                          //         note: L3 is the guard of the while loop                                      
   GETBP; CSTI 1; ADD; LDI   //         note: compute stack address of i and load i
   GETBP; LDI                //         note: compute stack address of n and load n
   LT                        // i < n;  note: guard of the while loop
   IFNZRO "L2"               //         note: if i < n, repeat from loop body
   RET 1     
```
Executing the command `java Machinetrace CEx/ex3.out 4` yields the following trace of the stack:
```fs
[ ]{0: LDARGS}
[ 4 ]{2: CALL 1 6}
[ 5 -999 4 ]{6: INCSP 1}
[ 5 -999 4 0 ]{8: GETBP}         // 0 is the value of i, i is at addr 3
[ 5 -999 4 0 2 ]{9: CSTI 1}      // add 1 to stack
[ 5 -999 4 0 2 1 ]{11: ADD}      // bp + 1 = address of i
[ 5 -999 4 0 3 ]{12: CSTI 0}     // add 0 to the stack
[ 5 -999 4 0 3 0 ]{14: STI}      // s[3] = 0, i.e. i = 0;
[ 5 -999 4 0 0 ]{15: INCSP -1}   // remove the value stored from sti
[ 5 -999 4 0 ]{17: GOTO 42}      // 
[ 5 -999 4 0 ]{42: GETBP}        // add bp to the stack
[ 5 -999 4 0 2 ]{43: CSTI 1}     // add 1 to the stack
[ 5 -999 4 0 2 1 ]{45: ADD}      // compute bp + 1, which is the addr of i
[ 5 -999 4 0 3 ]{46: LDI}        // load the value of i to the stack
[ 5 -999 4 0 0 ]{47: GETBP}      // add the bp to the stack
[ 5 -999 4 0 0 2 ]{48: LDI}      // load the value stored at bp, which is n = 4
[ 5 -999 4 0 0 4 ]{49: LT}       // push the result of 0 < 4 to the stack, 0 if false, 1 if true
[ 5 -999 4 0 1 ]{50: IFNZRO 19}  // since 0 < 4 it jumps to 19, which evaluates the body of the loop
[ 5 -999 4 0 ]{19: GETBP}        
[ 5 -999 4 0 2 ]{20: CSTI 1}
[ 5 -999 4 0 2 1 ]{22: ADD}
[ 5 -999 4 0 3 ]{23: LDI}        // 19-23 once again loads the values of i
[ 5 -999 4 0 0 ]{24: PRINTI}     // prints i
0 [ 5 -999 4 0 0 ]{25: INCSP -1} // removes value stored on the stack by print
[ 5 -999 4 0 ]{27: GETBP}        // add bp to the stack
[ 5 -999 4 0 2 ]{28: CSTI 1}     // add 1 to the stack
[ 5 -999 4 0 2 1 ]{30: ADD}      // add bp + 1 to the stack, addr of i
[ 5 -999 4 0 3 ]{31: GETBP}      // add bp to the stack
[ 5 -999 4 0 3 2 ]{32: CSTI 1}   // add 1 to the stack
[ 5 -999 4 0 3 2 1 ]{34: ADD}    // bp + 1, addr of i
[ 5 -999 4 0 3 3 ]{35: LDI}      // load value stored at 3 (addr of i)
[ 5 -999 4 0 3 0 ]{36: CSTI 1}   // add 1 to the stack
[ 5 -999 4 0 3 0 1 ]{38: ADD}    // add 1 to i, 0 + 1 = 1
[ 5 -999 4 0 3 1 ]{39: STI}      // store the value 1 at addr 3, i = 1
[ 5 -999 4 1 1 ]{40: INCSP -1}   // cleanup
[ 5 -999 4 1 ]{42: GETBP}        
[ 5 -999 4 1 2 ]{43: CSTI 1}
[ 5 -999 4 1 2 1 ]{45: ADD}        
[ 5 -999 4 1 3 ]{46: LDI}        // 42-46, compute addr of i and loads its value
[ 5 -999 4 1 1 ]{47: GETBP}     
[ 5 -999 4 1 1 2 ]{48: LDI}      // 47-48, load value of n
[ 5 -999 4 1 1 4 ]{49: LT}       // i < n = 1 < 4?
[ 5 -999 4 1 1 ]{50: IFNZRO 19}  // since 1 < 4, we jump to 19
[ 5 -999 4 1 ]{19: GETBP}        
[ 5 -999 4 1 2 ]{20: CSTI 1}
[ 5 -999 4 1 2 1 ]{22: ADD}      
[ 5 -999 4 1 3 ]{23: LDI}       // 19-23, load value of i
[ 5 -999 4 1 1 ]{24: PRINTI}    // print i
1 [ 5 -999 4 1 1 ]{25: INCSP -1}// remove result of print
[ 5 -999 4 1 ]{27: GETBP}      
[ 5 -999 4 1 2 ]{28: CSTI 1}
[ 5 -999 4 1 2 1 ]{30: ADD}     // 27-30, push addr of ito the stack
[ 5 -999 4 1 3 ]{31: GETBP}     
[ 5 -999 4 1 3 2 ]{32: CSTI 1}  
[ 5 -999 4 1 3 2 1 ]{34: ADD}  
[ 5 -999 4 1 3 3 ]{35: LDI}     // 31-35, load value of i
[ 5 -999 4 1 3 1 ]{36: CSTI 1} 
[ 5 -999 4 1 3 1 1 ]{38: ADD}
[ 5 -999 4 1 3 2 ]{39: STI}     // 35-39, i = i + 1, *[3] = 1 + 1, i = 2 
[ 5 -999 4 2 2 ]{40: INCSP -1}  // remove result of the assignment
[ 5 -999 4 2 ]{42: GETBP}
[ 5 -999 4 2 2 ]{43: CSTI 1}
[ 5 -999 4 2 2 1 ]{45: ADD}
[ 5 -999 4 2 3 ]{46: LDI}       // load value of variable i
[ 5 -999 4 2 2 ]{47: GETBP}     
[ 5 -999 4 2 2 2 ]{48: LDI}     // load vavlue of variable n
[ 5 -999 4 2 2 4 ]{49: LT}      
[ 5 -999 4 2 1 ]{50: IFNZRO 19} // i < n, 2 < 4? since yes, we jump to 19, again this is the guard
[ 5 -999 4 2 ]{19: GETBP}       // we know enter the body of the while loop once again, this has been traced above and the logic is the same
[ 5 -999 4 2 2 ]{20: CSTI 1}
[ 5 -999 4 2 2 1 ]{22: ADD}
[ 5 -999 4 2 3 ]{23: LDI}
[ 5 -999 4 2 2 ]{24: PRINTI}
2 [ 5 -999 4 2 2 ]{25: INCSP -1}
[ 5 -999 4 2 ]{27: GETBP}
[ 5 -999 4 2 2 ]{28: CSTI 1}
[ 5 -999 4 2 2 1 ]{30: ADD}
[ 5 -999 4 2 3 ]{31: GETBP}
[ 5 -999 4 2 3 2 ]{32: CSTI 1}
[ 5 -999 4 2 3 2 1 ]{34: ADD}
[ 5 -999 4 2 3 3 ]{35: LDI}
[ 5 -999 4 2 3 2 ]{36: CSTI 1}
[ 5 -999 4 2 3 2 1 ]{38: ADD}
[ 5 -999 4 2 3 3 ]{39: STI}
[ 5 -999 4 3 3 ]{40: INCSP -1}
[ 5 -999 4 3 ]{42: GETBP}         // again we enter the guard of the loop, i wont trace it again, since its the same logic
[ 5 -999 4 3 2 ]{43: CSTI 1}
[ 5 -999 4 3 2 1 ]{45: ADD}
[ 5 -999 4 3 3 ]{46: LDI}
[ 5 -999 4 3 3 ]{47: GETBP}
[ 5 -999 4 3 3 2 ]{48: LDI}
[ 5 -999 4 3 3 4 ]{49: LT}
[ 5 -999 4 3 1 ]{50: IFNZRO 19}  // 3 < 4, i < n? since it is, we go into loop again
[ 5 -999 4 3 ]{19: GETBP}
[ 5 -999 4 3 2 ]{20: CSTI 1}
[ 5 -999 4 3 2 1 ]{22: ADD}
[ 5 -999 4 3 3 ]{23: LDI}
[ 5 -999 4 3 3 ]{24: PRINTI}
3 [ 5 -999 4 3 3 ]{25: INCSP -1}
[ 5 -999 4 3 ]{27: GETBP}
[ 5 -999 4 3 2 ]{28: CSTI 1}
[ 5 -999 4 3 2 1 ]{30: ADD}
[ 5 -999 4 3 3 ]{31: GETBP}
[ 5 -999 4 3 3 2 ]{32: CSTI 1}
[ 5 -999 4 3 3 2 1 ]{34: ADD}
[ 5 -999 4 3 3 3 ]{35: LDI}
[ 5 -999 4 3 3 3 ]{36: CSTI 1}
[ 5 -999 4 3 3 3 1 ]{38: ADD}
[ 5 -999 4 3 3 4 ]{39: STI}
[ 5 -999 4 4 4 ]{40: INCSP -1}
[ 5 -999 4 4 ]{42: GETBP}
[ 5 -999 4 4 2 ]{43: CSTI 1}
[ 5 -999 4 4 2 1 ]{45: ADD}
[ 5 -999 4 4 3 ]{46: LDI}
[ 5 -999 4 4 4 ]{47: GETBP}
[ 5 -999 4 4 4 2 ]{48: LDI}
[ 5 -999 4 4 4 4 ]{49: LT}
[ 5 -999 4 4 0 ]{50: IFNZRO 19}  // 4 < 4, i < n? it isnt, so we return
[ 5 -999 4 4 ]{52: RET 1}        
[ 4 ]{5: STOP}
```

Some interesting points to observe is the while loop, especially how we immediately evaluate the guard, then evaluate the body if the guard holds, and then repeat this process.

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
