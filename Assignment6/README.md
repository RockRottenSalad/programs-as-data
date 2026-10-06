# Exercise 7.4
> 1) Extend the micro-C abstract syntax in Absyn.fs with the preincre-
ment and predecrement operators known from C, C++, Java, and C#
> ```fs
> type expr =
> ...
> | PreInc of access (* C/C++/Java/C# ++i or ++a[e] *)
> | PreDec of access (* C/C++/Java/C# --i or --a[e] *)
> ```

This was added to the abstract syntax in `Absyn.fs`.

> 2. Modify the micro-C interpreter in Interp.fs to handle PreInc and PreDec.
     You will need to modify the eval function, and use the getSto and setSto store
     operations (Sect. 7.3).

To do this, we added the missing match arms in the `eval` function:
```fs
    ...
    | PreInc acc ->
        let (addr, store1) = access acc locEnv gloEnv store
        let v = getSto store addr
        (v + 1, setSto store1 addr (v + 1))
    | PreDec acc ->
        let (addr, store1) = access acc locEnv gloEnv store
        let v = getSto store addr
        (v - 1, setSto store1 addr (v - 1))
```
The way this work is that first weevaluate the `access` of the value in the `PreInc/Predec` constructor. This yields us an address and a store. Then we look up the value at `addr` in the store.
Since it is post increment and decrement, we return the updated value and a new store, where the valuea at `addr` has been updated aswell.

# Exercise 7.5 
> Extend the micro-C lexer and parser to accept ++e and –e also, and
to build the corresponding abstract syntax.

The first step in doing this is extending the lexer to recognize `"++"` and `"--""` as the tokens `PLUSPLUS` and `MINUSMINUS`:
```
  ...
  | "++"            { PLUSPLUS }
  | "--"            { MINUSMINUS }
```
Inside the parser (`CPar.fsy`), we specify `PLUSPLUS` and `MINUSMINUS` as tokens:
```
%token PLUSPLUS MINUSMINUS         /* changed 7.5 */
```

We have to also specify their precedence. Both `++` and `--` behave exactly the same way as `NOT` and `AMP`. It must have higher precende than binary operators, because we want `++x + y` to parse as `(++x) + y` and not `++(x + y)`. We must however have it below `LBRACK` because we want `++a[i]` to parse as `++(a[i])`. So we end up with:
```
%right ASSIGN             /* lowest precedence */
%nonassoc PRINT
%left SEQOR
%left SEQAND
%left EQ NE 
%left GT LT GE LE
%left PLUS MINUS
%left TIMES DIV MOD 
%nonassoc NOT AMP
%nonassoc PLUSPLUS MINUSMINUS /* changed  */
%nonassoc LBRACK          /* highest precedence  */
```
Now the last thing left to do is to add the production rules for `PreInc` and `PreDec` to the expressions of our parser.
```
ExprNotAccess:
  ...
  | PLUSPLUS Access                     { PreInc($2)           } /* changed */
  | MINUSMINUS Access                   { PreDec($2)           } /* changed */
;
```


# Exercise 8.1
> Download microc.zip from the book homepage, unpack it to a
folder MicroC, and build the micro-C compiler as explained in README.TXT step
(B).
> 
> (i) As a warm-up, compile one of the micro-C examples provided, such as that in
source file ex11.c, then run it using the abstract machine implemented in Java,
as described also in step (B) of the README file.

Okay, done.

> (ii) Now compile the example micro-C programs ex3.c and ex5.c using functions
compileToFile and fromFile from ParseAndComp.fs as above. Study the generated symbolic bytecode. Write up the bytecode in a more structured
way with labels only at the beginning of the line (as in this chapter). Write the
corresponding micro-C code to the right of the stack machine code.

**NOTE:** Please note that the bytecode for exercise 3 was generated using the `Contcomp.fs` compiler rather than the `Comp.fs` compiler by mistake. We did not want to rewrite the entirety of the exercise to change this, but now you know.

When executing `compileToFile (fromFile "CEx/ex05.c") "CEx/ex5.out";;` we get the following symbolic bytecode:
```fsharp
> compileToFile (fromFile "CEx/ex05.c") "CEx/ex5.out";;
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

# Exercise 8.2
> Compile and run the micro-C example programs you wrote in
Exercise 7.2, and check that they produce the right result. It is rather cumbersome
to fill an array with values by hand in micro-C, so the function squares from that
exercise is very handy.

Okay, done. The programs can be found in `Peak/i.c`, `Peak/ii.c` and `Peak/iii.c`. Below the results of some commands are seen:
```
java Machine Peak/i.out
> 37

java Machine Peak/ii.out 10
> 285

java Machine Peak/ii.out
> 1 4 2 0
```

# Exercise 8.3
> This abstract syntax for preincrement ++e and predecrement --e was introduced in Exercise 7.4:
> ```
> type expr =
> ...
> | PreInc of access (* C/C++/Java/C# ++i or ++a[e] *)
> | PreDec of access (* C/C++/Java/C# --i or --a[e] *)
> ```
> Modify the compiler (function cExpr) to generate code for PreInc(acc) and
> PreDec(acc). To parse micro-C source programs containing these expressions,
>you also need to modify the lexer and parser.

It is important that we don't expand `++e` to `e = e + 1`, but this evalutes `e` twice, which does not work, as `e` might have a side effect itself, like `++arr[++i]`. A simple way of avoiding this is to do the following:
```

command        stack          note
access E       [3]            address of E
DUP            [3; 3]         duplicate addr, so we remember the address later for STI
LDI            [3; 0]         load the current value at address E
CSTI 1         [3; 3; 1]        
ADD            [3; 4]         E + 1
STI            [4]            E = E + 1, i.e. store value for at address 3    
```
Now this only computes `E`, i.e. when we compute its address, so we don't have issues if `E` has side effects. This approach leaves the
updated value on the stack, as one would expect. To make it work for `--`, we simply change `ADD` to `SUB`. In `cExpr` we then add:
```fs
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    ...
    | PreInc acc ->
        cAccess acc varEnv funEnv
        @ [DUP; LDI; CSTI 1; ADD; STI]
    | PreDec acc ->
        cAccess acc varEnv funEnv
        @ [DUP; LDI; CSTI 1; SUB; STI]
```
We must also modify the compiler that uses continuations:
```fs
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) (c : instr list) : instr list =
    ...
    | PreInc acc -> cAccess acc varEnv funEnv (DUP :: LDI :: CSTI 1 :: ADD :: STI :: c) // CHANGED
    | PreDec acc -> cAccess acc varEnv funEnv (DUP :: LDI :: CSTI 1 :: SUB :: STI :: c) // CHANGED
```

We can write the new rules as:
```
Instruction         Stack Before         Stack After         Effect
PREINC              s, v                 s, s[v] + 1         s[v] = s[v] + 1
PREDEC              s, v                 s, s[v] - 1         s[v] = s[v] - 1
```

To check that this works we can write the following program:
```cs
void main() 
{
void main() 
{
    int i;
    i = 0;
    int a[4];
    a[0] = 1;
    a[0] = 2;
    a[0] = 3;
    a[0] = 4;
    
    // ++i has a side effect, and we want it to execute only once, i.e. its not ok to do
    // arr[++i] = arr[++i] + 1
    // after the expression we want i to be 1, and arr[1] to be 3
    print ++a[++i]; // we would expect this to print  3
    print i;          // should print 1
    
    // here i should be 0 and arr[0] should be 0
    print --a[--i]; // we would expect this to print  0
    print i;          // should print 0
}
}
```
The implementation works both when using `Comp.fs` and `Contcomp.fs`. We used the following command to start interactive:
```
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Contcomp.fs ParseAndComp.fs
```

And then running:

```fs
> open ParseAndComp;;
> compileToFile (fromFile "Peak/pretest.c") "Peak/pretest.out";;  
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 0; STI;
   INCSP 3; GETSP; CSTI 3; SUB; GETBP; CSTI 5; ADD; LDI; CSTI 1; STI; INCSP -1;
   GETBP; CSTI 5; ADD; LDI; CSTI 2; STI; INCSP -1; GETBP; CSTI 5; ADD; LDI;
   CSTI 3; STI; INCSP -1; GETBP; CSTI 5; ADD; LDI; CSTI 4; STI; INCSP -1;
   GETBP; CSTI 5; ADD; LDI; GETBP; DUP; LDI; CSTI 1; ADD; STI; ADD; DUP; LDI;
   CSTI 1; ADD; STI; PRINTI; INCSP -1; GETBP; LDI; PRINTI; INCSP -1; GETBP;
   CSTI 5; ADD; LDI; GETBP; DUP; LDI; CSTI 1; SUB; STI; ADD; DUP; LDI; CSTI 1;
   SUB; STI; PRINTI; INCSP -1; GETBP; LDI; PRINTI; RET 6]
```
We can then run the code using the Java machine:
```
java Machine Peak/pretest.out
> 3 1 0 0
```
This prints the expected `3 1 0 0`. Please refer to the comments in the example to understand why this is expected.


# Exercise 8.4
> (i) Compile ex8.c and study the symbolic bytecode to see why it is so
much slower than the handwritten 20 million iterations loop in prog1.

This is `Prog1`:
```
0 20000000 70 7 0 1 11 30 72 4 101
```
The following is the symbolic bytecode given from compiling `ex08.out` using the `Comp.fs` compiler:
```fs
> compileToFile (fromFile "CEx\ex08.c") "CEx/ex08.out";;    
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 0; ADD;
   CSTI 20000000; STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 0; ADD;
   GETBP; CSTI 0; ADD; LDI; CSTI 1; SUB; STI; INCSP -1; INCSP 0; Label "L3";
   GETBP; CSTI 0; ADD; LDI; IFNZRO "L2"; INCSP -1; RET -1]
```

> (ii) Compile ex13.c and study the symbolic bytecode to see how loops and condi-
tionals interact; describe what you see.

```fs
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD; CSTI 0; EQ; IFZERO "L7"; GETBP; CSTI 1; ADD; LDI; CSTI 100;
   MOD; CSTI 0; EQ; NOT; IFNZRO "L9"; GETBP; CSTI 1; ADD; LDI; CSTI 400; MOD;
   CSTI 0; EQ; GOTO "L8"; Label "L9"; CSTI 1; Label "L8"; GOTO "L6";
   Label "L7"; CSTI 0; Label "L6"; IFZERO "L4"; GETBP; CSTI 1; ADD; LDI;
   PRINTI; INCSP -1; GOTO "L5"; Label "L4"; INCSP 0; Label "L5"; INCSP 0;
   Label "L3"; GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD; LDI; LT;
   IFNZRO "L2"; INCSP -1; RET 0]
```

