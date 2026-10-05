# exercise 7.1
> Extend the micro-C abstract syntax in Absyn.fs with the preincrement and predecrement operators [...] 
> Modify the micro-C interpreter in Interp.fs to handle PreInc and PreDec.
> You will need to modify the eval function, and use the getSto and setSto store operations

introduce the mentioned changes to `Absyn.fs`:
````fsharp
type expr =
  ...
  | PreInc of access                 (* C/C++/Java/C# ++i or ++a[e] CHANGED *)
  | PreDec of access                 (* C/C++/Java/C# --i or --a[e] CHANGED *)
````
then change eval to the following:
```Fsharp
| PreInc(acc) ->                                                (* CHANGED *)
        let (loc, store1) = (access acc locEnv gloEnv store)
        let newValue = (getSto store1 loc) + 1
        (newValue, setSto store1 loc newValue)
    | PreDec(acc) ->                                            (* CHANGED *)
        let (loc, store1) = (access acc locEnv gloEnv store)
        let newValue = (getSto store1 loc) + 1
        (newValue, setSto store1 loc newValue)
```
the idea is to first find the addresse of ``acc`` in ``store``.
Then find the value of ``acc`` in ``store`` using getSto,
and increase/decrease it by one. 
lastly add the new updated value to the ``store``.
# Exercise 7.5
no new tokens to be added. plus and minus tokens already exists. We only need to add this change to the parser:
````fsharp
ExprNotAccess:
  ...
  | PLUS PLUS Expr                      { PreInc($3) }          /* CHANGED */
  | MINUS MINUS Expr                    { PreInc($3) }          /* CHANGED */
;
````
here is a simply testcase to ensure it works:
````fsharp
void main()
{
    int i;
    i = 0;
    while (i < 10)
    {
        print ++i;
    }
    println;
}
````
now lets build the project:
````fsharp
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll
Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs
````
and test that it works as intended:
````fsharp
> open ParseAndRun;;
> run (fromFile "test/test1.c")[];; 
1 2 3 4 5 6 7 8 9 10                                                                                                                                
val it: Interp.store = map [(0, 10)]
````
# exercise 8.1
>(i) As a warm-up, compile one of the micro-C examples provided, such as that in
source file ex11.c, then run it using the abstract machine implemented in Java,
as described also in step (B) of the README file. When run with command line
argument 8, the program prints the 92 solutions to the eight queens problem:
how to place eight queens on a chessboard so that none of them can attack any
of the others.

been there, done that.

>(ii) Now compile the example micro-C programs ex3.c and ex5.c using
functions compileToFile and fromFile from ParseAndComp.fs as
above.
Study the generated symbolic bytecode. Write up the bytecode in a more struc-
tured way with labels only at the beginning of the line (as in this chapter). Write
the corresponding micro-C code to the right of the stack machine code. Note that
ex5.c has a nested scope (a block { ... } inside a function body); how is that
visible in the generated code?

by executing ``compileToFile (fromFile "CEx/ex05.c") "CEx/ex5.out";;`` we get the following:
````fsharp
> compileToFile (fromFile "CEx/ex05.c") "CEx/ex05.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   GETBP; CSTI 0; ADD; LDI; STI; INCSP -1; INCSP 1; GETBP; CSTI 0; ADD; LDI;
   GETBP; CSTI 2; ADD; CALL (2, "L2"); INCSP -1; GETBP; CSTI 2; ADD; LDI;
   PRINTI; INCSP -1; INCSP -1; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1;
   INCSP -1; RET 0; Label "L2"; GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD;
   LDI; GETBP; CSTI 0; ADD; LDI; MUL; STI; INCSP -1; INCSP 0; RET 1]
````
here is an explanation of the byte code, 
it is recommended to read this while having ex05.c in view:
````fsharp
LDARGS 1;          //adds argument to stack
CALL (1, "L1");    //calls main
STOP;              //end after
Label "L1";          //start of main
INCSP 1;             //int r;
GETBP; CSTI 1; ADD;  //push r to address bp +1
GETBP; CSTI 0; ADD;  //push n to address bp +0
LDI;                 //get rvalue at address n
STI;                 //store value n in address r
INCSP -1;            //pop leftover
INCSP 1;                    //inner block, int r;
GETBP; CSTI 0; ADD; LDI;    //push arg 1  (the order might be revered)
GETBP; CSTI 2; ADD;         //push arg 2  (check later #TODO!!!!)
CALL (2, "L2");             //call square(...)
INCSP -1;            
GETBP; CSTI 2; ADD; LDI;    //push r
PRINTI;                     //print inner r
INCSP -1;                 
INCSP -1;                   //end of block
GETBP; CSTI 1; ADD; LDI; //push r
PRINTI;                  //print outer r
INCSP -1;
INCSP -1;
RET 0;                   //end of function, return
Label "L2";              //start of square(...)
GETBP; CSTI 1; ADD; LDI; //push rp
GETBP; CSTI 0; ADD; LDI; //push i
GETBP; CSTI 0; ADD; LDI; //push i again
MUL;                     //multiply i with i
STI;                     //store the value i*i in rp
INCSP -1;
INCSP 0;
RET 1                    //return
````
[explain how the inner block is visible]
# exercise 8.3
>Modify the compiler (function cExpr) to generate code for PreInc(acc) and
PreDec(acc).

we introduce the following changes to ``cExpr``:
````fsharp
    and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    ...
    | PreInc acc     -> cAccess acc varEnv funEnv @ [DUP; LDI; CSTI 1; ADD; STI] (* CHANGED *)
    | PreDec acc     -> cAccess acc varEnv funEnv @ [DUP; LDI; CSTI 1; SUB; STI] (* CHANGED *)
````
now lets test it. on the following code:
````fsharp
void main()
{
    int i;
    i = -1;
    int arr[3];
    arr[0] = 6;
    arr[1] = 7;
    arr[2] = 8;
    while (i < 2)
    {
        print ++arr[++i];
    }
    println;
}
````
the expected result is that it prints ``7 8 9``.
````fsharp
> open ParseAndComp;;
> compileToFile (fromFile "test/test2.c") "test/test2.out";;
````
now we run it through the machine:
````
$ java Machine.java test/test2.out
7 8 9
Used 0.001 seconds
````
# exercise 8.4
no :)