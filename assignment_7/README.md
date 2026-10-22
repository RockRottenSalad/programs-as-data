# Exercise 8.5
> Extend the micro-C language, the abstract syntax, the lexer, the parser,
and the compiler to implement conditional expressions of the form (e1 ? e2 :
e3).

First we update our AST in `Absyn.fs` so that we have support for the ternary operator.

```fsharp
type typ =
...
and expr =                                                         
...
  | Ternary of expr * expr * expr    (* Ternary operator CHANGED | 8.5 *)
```

Then we update the tokenizer in `CLex.fsl` to accept `?` and `:`.

```fsharp
rule Token = parse
...
  | '?'             { QUESTION }
  | ':'             { COLON }
...
```

Now we need to register the tokens in the parser.

```
%token COLON QUESTION
```

We also need to ensure that the precedence is correct. We want all binary
operators and unary operators to bind tigther than `?` and `:`.

```
%right ASSIGN             /* lowest precedence */
%nonassoc PRINT
%left QUESTION COLON /* CHANGED | 8.5 */
%left SEQOR
...
%nonassoc LBRACK          /* highest precedence  */
```

Finally we just update the compiler to handle the ternary operator. We
simply compile the three expressions, add two labels and depending on
what the condition expression evaluates to, we jump to the correct label.

```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    match e with
    ...
    | Ternary(condition, e1, e2) ->
      let compiledCondition = cExpr condition varEnv funEnv
      let compiledE1 = cExpr e1 varEnv funEnv
      let compiledE2 = cExpr e2 varEnv funEnv
      let labtrue   = newLabel()
      let labend   = newLabel()

      compiledCondition
      @ [IFNZRO labtrue]
      @ compiledE2
      @ [GOTO labend; Label labtrue]
      @ compiledE1
      @ [Label labend]
```

Now we check that this is works as intended. We do so by writing a simple test program in `sanity/ternary2.c`.

Now we make sure our lexer and parser are up to date and we spin up the compiler in an interactive environment.

```
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll \
    Absyn.fs CPar.fs CLex.fs Parse.fs \
    Machine.fs Comp.fs ParseAndComp.fs
```

Now that we have our compiler running, we can compile the test program like so.

```fsharp
> open ParseAndComp;;
> let pro = fromFile "sanity/ternary2.c";;
val pro: Absyn.program =
  Prog
    [Fundec
       (None, "main", [(TypI, "n")],
        Block
          [Stmt
             (Expr
                (Prim1
                   ("printi",
                    Ternary
                      (Prim2
                         (">=",
                          Prim2
                            ("-", Prim2 ("+", Access (AccVar "n"), CstI 2),
                             CstI 2), Prim2 ("*", CstI 25, CstI 0)),
                       Prim2 ("-", CstI 5, CstI 4),
                       Prim2 ("-", CstI 3, CstI 3)))));
           Stmt (Expr (Prim1 ("println", CstI 10)))])]

> compileToFile pro "ternary_test";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; GETBP; CSTI 0; ADD; LDI; CSTI 2;
   ADD; CSTI 2; SUB; CSTI 25; CSTI 0; MUL; LT; NOT; IFNZRO "L2"; CSTI 3;
   CSTI 3; SUB; GOTO "L3"; Label "L2"; CSTI 5; CSTI 4; SUB; Label "L3"; PRINTI;
   INCSP -1; CSTI 10; PRINTNL; INCSP -1; INCSP 0; RET 0]

```

Finally we run the program and see that we get the expected result.

```
$ java Machine.java ternary_test 23
1

Used 0.001 seconds

$ java Machine.java ternary_test -23
0

Used 0.002 seconds
```


For the sake of completeness, we also updated the interpreter.

```
and eval e locEnv gloEnv store : int * store = 
...
    | Ternary(condition, e1, e2) ->
      let (i1, store1) = eval condition locEnv gloEnv store
      if i1 <> 0
      then eval e1 locEnv gloEnv store1
      else eval e2 locEnv gloEnv store1
```

# Exercise 8.6 
> Extend the lexer, parser, abstract syntax and compiler to implement switch statements.

First we define a switch statement in our AST in `Absyn.fs`. The idea is the
first `expr` is the value we're switching on. Then we have a list of `(int *
stmt)` pairs where the int is the case value and the `stmt` is the body of the
case.

```fsharp
and stmt =                                                         
  ...
  | Switch of expr * (int * stmt) list         (* Switch statement CHANGED | 8.6 *)
```

Then we introduce `switch` and `case` as keywords inside the lexer in `CLex.fsl`.

```fsharp
let keyword s =
    match s with
    ...
    | "switch"  -> SWITCH (* CHANGED | 8.6 *)         
    | "case"    -> CASE   (* CHANGED | 8.6 *)         
```

We also introduce the colon character as a token in the lexer.

```fsharp
rule Token = parse
  ...
  | ':'             { COLON }
```

Finally we need to update the function for compiling statements in `Comp.fs` to handle our new AST node `Switch`.

The idea here is simple.

1. We generate a label that is placed at the very end of the switch statement
2. We compile the value being switched on, effectively leaving it on top of the stack.
3. For each case:
    - Generate a label that is placed at the very end of the case so that the next instruction would either be the next case or the end of the switch statement.
    - Duplicate the switch value(which is currently on top of the stack).
    - Compile the case value so that it appears on top of the stack.
    - Check for equality.
    - If the values are not equal, jump to the skip case label generated earlier.
    - Generate the body of the case.
    - Jump to the end of the switch statement using the label generated in step 1.
4. Now the switch statement is over, we pop off 1 element from the stack. This is the value that was being switched on.

```fsharp
let rec cStmt stmt (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    ...
    | Switch(v, cases) ->
      let endLabel = newLabel()
      let v' = cExpr v varEnv funEnv
      let cases' = List.fold (fun compiled (case, body) -> 
          let skipCaseLabel = newLabel()
          let case' = cExpr (CstI case) varEnv funEnv
          let body' = cStmt body varEnv funEnv

          compiled
          @ [DUP] (* value being switched on must be on top of the stack *)
          @ case'
          @ [EQ; IFZERO skipCaseLabel]
          @ body' 
          @ [GOTO endLabel; Label skipCaseLabel]) [] cases
      v' @ cases' @ [Label endLabel; INCSP -1] (* popping off v *)
```
Now we want to test that this works. We've written a simple test case in `sanity/switch.c`.

```c
void main(int n) {
    int x;
    x = -1;
    switch (n) {
        case 1:
             x = 1; 
        case 2:
            x = 2;
        case 3:
            x = 3;
    }
    print x;
    println;
}
```

First we make sure our parser and lexer is up to date and then we run the compiler in an interactive environment.
```
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll \
    Absyn.fs CPar.fs CLex.fs Parse.fs \
    Machine.fs Comp.fs ParseAndComp.fs

```

Now we compile the test program.
```fsharp
> open ParseAndComp;;
> let pro = fromFile "sanity/switch.c";;
val pro: Absyn.program =
  Prog
    [Fundec
       (None, "main", [(TypI, "n")],
        Block
          [Dec (TypI, "x"); Stmt (Expr (Assign (AccVar "x", CstI -1)));
           Stmt
             (Switch
                (Access (AccVar "n"),
                 [(1, Expr (Assign (AccVar "x", CstI 1)));
                  (2, Expr (Assign (AccVar "x", CstI 2)));
                  (3, Expr (Assign (AccVar "x", CstI 3)))]));
           Stmt (Expr (Prim1 ("printi", Access (AccVar "x"))));
           Stmt (Expr (Prim1 ("println", CstI 10)))])]

> compileToFile pro "switch_test";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI -1; STI; INCSP -1; GETBP; CSTI 0; ADD; LDI; DUP; CSTI 1; EQ;
   IFZERO "L3"; GETBP; CSTI 1; ADD; CSTI 1; STI; INCSP -1; GOTO "L2";
   Label "L3"; DUP; CSTI 2; EQ; IFZERO "L4"; GETBP; CSTI 1; ADD; CSTI 2; STI;
   INCSP -1; GOTO "L2"; Label "L4"; DUP; CSTI 3; EQ; IFZERO "L5"; GETBP;
   CSTI 1; ADD; CSTI 3; STI; INCSP -1; GOTO "L2"; Label "L5"; Label "L2";
   INCSP -1; GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; CSTI 10; PRINTNL;
   INCSP -1; INCSP -1; RET 0]
```


Finally we can run it using the stack machine implemented in Java and see that we get the correct result.
```
$ java Machine.java switch_test 1
1

Used 0.002 seconds

$ java Machine.java switch_test 2
2

Used 0.001 seconds

$ java Machine.java switch_test 3
3

Used 0.001 seconds

$ java Machine.java switch_test 4
-1

Used 0.002 seconds
```

# Exercise 9.1
JVM Bytecode explained:
```java
  public static void SelectionSort(int[]);
    descriptor: ([I)V
    flags: (0x0009) ACC_PUBLIC, ACC_STATIC
    Code:
      stack=4, locals=4, args_size=1
         0: iconst_0	
         1: istore_1    	// int i = 0
         2: iload_1     	// load value of i
         3: aload_0     	// load length of array
         4: arraylength 	// compute arraylength
         5: if_icmpge     57    // if arraylength >= i, i.e. (i < arraylength)
         8: iload_1             // load value of i which is at address 1
         9: istore_2            // least = value of i
        10: iload_1             // load value of i
        11: iconst_1            // push 1 to the stack
        12: iadd                // compute i + 1
        13: istore_3            // j = i + 1 (assign j to be that result, j is at address 3)
        14: iload_3             // load value of j
        15: aload_0             // load array
        16: arraylength         // compute arraylength and push to stack
        17: if_icmpge     37    // if arraylength >= j continue, i.e. (j < arraylength)
        20: aload_0             // load array
        21: iload_3             // load value of j
        22: iaload              // get array[j]
        23: aload_0             // get value of array
        24: iload_2             // get value of least
        25: iaload              // get array[least]
        26: if_icmpge     31    // if (array[least] >= array[j]), i.e. array[j] < array[least] else go to 31
        29: iload_3             // load value of j
        30: istore_2            // store j in least
        31: iinc          3, 1  // increment j
        34: goto          14    // repeat loop body
        37: aload_0             
        38: iload_1
        39: iaload
        40: istore_3
        41: aload_0
        42: iload_1
        43: aload_0
        44: iload_2
        45: iaload
        46: iastore
        47: aload_0
        48: iload_2
        49: iload_3
        50: iastore            // all the above do: int tmp = arr[i]; arr[i] = arr[least]; arr[least] = tmp;
        51: iinc          1, 1 // i = i + 1
        54: goto          2    // repeat loop body for i
        57: return             // return
}
```

# Exercise 9.3
> Fix QueueWithMistake.java

The memory leak stems from the get method.

This is the behaviour of the put method. It simply updates the tail pointer as well as making the Sentinel point to the new node.
```
put
Before
h   t
  S

After
h    t
S -> n
```

However, when we run the get method we naively update the head to point to `head.next`. The problem is that we still have access to our sentinel value and that points to the node that was supposedly removed, thus the garbage collector cannot free this node since it's still reachable.

```
get
Before
h        t
|-> S -> n

After
   h  t
S->  n  | n just leaked
```

The fix is to simply keep in mind that we're using a sentinel value for the head. So we just update the head's next pointer to `head.next.next` instead.

```java
public synchronized int get() {
    if (head.next == null) 
      return -999;

    // fix
    // Before
    // h        t
    // |-> S -> n
    //
    // After
    // h   t
    // |-> S
    int value = head.next.item;
    head.next = head.next.next;

    // When list is empty, both tail and head should point to the sentinel
    if(head.next == null) tail = head;

    return value;

    // mistake
    // Before
    // h        t
    // |-> S -> n
    //
    // After
    //     h  t
    // S->  n    | n just leaked
    //    Node first = head;
    //    head = first.next;
    //    return head.item;
}
```


