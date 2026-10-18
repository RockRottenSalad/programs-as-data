# Exercise 8.5
> Extend the micro-C language, the abstract syntax, the lexer, the parser, and the compiler to implement conditional expressions of the form (e1 ? e2 : e3).

Since the exercise requires the construction to leave either $e_2$ or $e_3$ on the stack, we must introduce a seperate expressions 
in our abstract syntax (`Absyn.fs`):
```fsharp
and expr =                                                         
  ...
  | Cond of expr * expr * expr       (* Changed *)
```
After introducing this we must also add functionality to `cExpr` in the compiler (`Comp.fs`):
```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    ...
    | Cond(e1, e2, e3) -> // changed
        let labfalse = newLabel()
        let labend = newLabel()
        cExpr e1 varEnv funEnv @ [IFZERO labfalse]
        @ cExpr e2 varEnv funEnv @ [GOTO labend]
        @ [Label labfalse] @ cExpr e3 varEnv funEnv
        @ [Label labend]
```
The implementation follows the exact same logic of the `if-else` compiler branch.

We must also modify the lexer to recognize the tokens `?` and `:`
```fsharp
rule Token = parse
  ...
  | ':'             { COLON } (* changed *)
  | '?'             { QMARK } (* changed *)
  ...
  | _               { failwith "Lexer error: illegal symbol" }
```
These tokens must be specificed in the parser:
```fsharp
%token QMARK COLON /* changed */
```
To make sure the it functions properly, we must also make sure it has lower precedence than `>` etc., since we would want `x > 5 ? 2 : 3` to parse as `(x > 5), 2, 3)` and not `x > (5 : 2 ? 3)`.
```fsharp
%right ASSIGN             /* lowest precedence */
%nonassoc PRINT
%left QMARK COLON /* changed */
%left SEQOR
%left SEQAND
%left EQ NE 
%left GT LT GE LE
%left PLUS MINUS
%left TIMES DIV MOD 
%nonassoc NOT AMP 
%nonassoc LBRACK          /* highest precedence  */
```
Lastly we must add a production rule in parser that produces `Comp(e1, e2, e3)`:
```fsharp
ExprNotAccess:
  ...
  | Expr QMARK  Expr COLON Expr         { Cond($1, $3, $5)     } /* changed */
;
```

To test the implementation please see the program in `Test/contcomp.c`. Compiling the program yields:
```fsharp
> compileToFile (fromFile ("Test/condtest.c")) "Test/condtest.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; GETBP; CSTI 0; ADD; LDI; CSTI 0;
   SWAP; LT; IFZERO "L2"; CSTI 1; GOTO "L3"; Label "L2"; CSTI 2; Label "L3";
   PRINTI; INCSP -1; INCSP 1; GETBP; CSTI 1; ADD; CSTI 5; STI; INCSP -1; GETBP;
   CSTI 0; ADD; LDI; GETBP; CSTI 1; ADD; LDI; CSTI 5; SWAP; LT; IFZERO "L6";
   GETBP; CSTI 1; ADD; LDI; GOTO "L7"; Label "L6"; CSTI 2; Label "L7"; SWAP;
   LT; IFZERO "L4"; CSTI 1; GOTO "L5"; Label "L4"; CSTI 2; Label "L5"; PRINTI;
   INCSP -1; INCSP -1; RET 0]
```
Which if run behaves as expected:
```fsharp
> java Machine.java .\Test\condtest.out -2
2 2
> java Machine.java .\Test\condtest.out 2
1 2
> java Machine.java .\Test\condtest.out 3
1 1
```

# Exercise 8.6
The first step was to modify the abstract syntax in `Absyn.fs` to have a constructor for a switch statement. Since a switch statement is of the form:
`switch (expr) { ... }`, it takes an `expr` and then it takes a list of integer constants and a stmt, which represents a case. Since `case <const>: {stmt block}`:
```fsharp
and stmt =                                                         
  ...
  | Switch of expr * (int * stmt) list
```
We then add `SWITCH` and `CASE` to the lexer and parser as tokens. Now we extend the `StmtM` to have production rulee for a switch statement:
```
StmtM:  /* No unbalanced if-else */
  ...
  | SWITCH LPAR Expr RPAR LBRACE Cases RBRACE         { Switch($3, $6)       } /* changed */
;

Cases: /* changed */
  /* empty */                           { [] }
  | CASE CSTINT COLON Block Cases       { ($2, $4) :: $5 }
  | CASE MINUS CSTINT COLON Block Cases { (- $3, $5) :: $6 }
;
```

The compiler rule we have implemented is quite overwhelming, so here it is described in smaller bites:
```fsharp
    | Switch(e, cs) -> // changed
        ...
        cExpr e varEnv funEnv ...
```
The first thing we do is push the value of the expression inside `switch()` to the stack. Afterwards we recursively handle each case, using this logic:
```fs
    | Switch(e, cs) -> // changed
        let endLabel = newLabel()
        let rec aux =
            function
            | [] -> []
            | (x, body) :: rest ->
                let next = newLabel()
                [DUP; CSTI x; EQ; IFZERO next]
                @ [INCSP -1] @ cStmt body varEnv funEnv @ [GOTO endLabel] // we pop the duplicate value before executing the case
                @ [Label next] @ aux rest
        cExpr e varEnv funEnv @ aux cs ...
```
The idea is: First duplicate the value currently on the stack, which is whatever the expression inside switch evaluated to. Then push the integer constant to the stack, and check if they are equal. If they are not equal and the resulting value on the stack is `0`, we remove this value and go to the next case by jumping to next. If the value is `1`, we then remove the copy of `e`, and then evaluate the statement of the case. After evaluating this, we are done and jump to the end label. 

Last logic to add is what happens if we fall through all cases. Then we will have the copy of `e` on the stack, and we must remove it, leaving us with:
```fsharp
    | Switch(e, cs) -> // changed
        let endLabel = newLabel()
        let rec aux =
            function
            | [] -> []
            | (x, body) :: rest ->
                let next = newLabel()
                [DUP; CSTI x; EQ; IFZERO next]
                @ [INCSP -1] @ cStmt body varEnv funEnv @ [GOTO endLabel] // we pop the duplicate value before executing the case
                @ [Label next] @ aux rest
        cExpr e varEnv funEnv @ aux cs @ [INCSP -1; Label endLabel] // we pop one value off the stack incase we feell through all cases
```



This can be tested using the `Test/switchtest.c` program, which can be seen executeed below:
```
> java Machine.java .\Test\switchtest.out 1
1
> java Machine.java .\Test\switchtest.out 2
2
> java Machine.java .\Test\switchtest.out 3
3
> java Machine.java .\Test\switchtest.out 4
<nothing>
```

# Exercise 9.1

> (i) From a Visual Studio Command Prompt, compile it using Microsoft’s C# com-
piler with the optimize flag (/o), then disassemble it, saving the output to file
Selsort.il:
> 
> (ii) Now do the same with the corresponding Java method in file Selsort.java.
Compile it, then disassemble the Selsort class:

Please see the files with comments inside of `path`.

# 9.3

Dummy points tot he first node created which is the sentinel. When we remove an item, dummy will always point to the removed head, so there is a permanent reference, which means that we garbage collector can free the memory.

We can fix this by simply not keeping a permanent reference to dummy in the class, please see `QueueWithFix.java`. Below is a demonstration that the fix works.
```java
> javac QueueWithMistake.java
> java -Xmx256m -Xlog:gc QueueWithMistake
...
Exception in thread "main" java.lang.OutOfMemoryError: Java heap space
        at QueueWithMistake.runThreads(QueueWithMistake.java:58)
        at QueueWithMistake.main(QueueWithMistake.java:28)
        
> javac QueueWithFix.java
> java -Xmx256m -Xlog:gc QueueWithFix
[0.005s][info][gc] Using G1
[0.030s][info][gc] GC(0) Pause Young (Normal) (G1 Evacuation Pause) 5M->1M(10M) 0.428ms
[0.035s][info][gc] GC(1) Pause Young (Normal) (G1 Evacuation Pause) 4M->1M(10M) 0.240ms
...
```
The fixed version runs fine without runniong out of memory.