# 8.5
here is the changes made to the abstract syntax:
```F#
  and expr = 
  ...
  | Iffy of expr * expr * expr       (* CHANGED                     *)
```
the lexer:
```fsharp
rule Token = parse
  ...
  | ':'             { COLON }                        (* CHANGED *)
  | '?'             { QUEST }                        (* CHANGED *)
  ...
```
the parser:
```
ExprNotAccess:
  ...
  | Expr QUEST Expr COLON Expr          { Iffy($1, $3, $5)     }
;
```
```
%left SEMIC QUEST        /* CHANGED */
```
and the compiler:
```fsharp
and cExpr (e : expr) (varEnv : varEnv) (funEnv : funEnv) : instr list = 
    ...
    | Iffy (e1, e2, e3)->
      let labelse = newLabel()
      let labend  = newLabel()
      cExpr e1 varEnv funEnv @ [IFZERO labelse] 
      @ cExpr e2 varEnv funEnv @ [GOTO labend]
      @ [Label labelse] @ cExpr e3 varEnv funEnv
      @ [Label labend]
```
the following shows how to run the test case:
```shell
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs
```
```
open ParseAndComp;;
compileToFile (fromFile "tests/test1.c") "tests/test1.out";;
#q;;
```
```shell
java Machine.java
java Machine tests/test1.out
```
here is the test case:
```C
void main() {
    int x;
    x = 0;
    true ? x = x + 1 : x = x - 1;
    print x;
    (false ? x = x - 1 : (true ? x = x + 1 : x = x - 1));
    print x;
    println;
}
```
and this is the output, which matches the expected output:
```
1 2 
```
# 8.6
here is the changes made to the abstract syntax:
```F#
and stmt =
  ...
  | Switch of expr * (int * stmt) list (* CHANGED *)
```
the lexer:
```fsharp
let keyword s =
    match s with
    ...
    | "switch"  -> SWITCH        (* CHANGED *)
    | "case"    -> CASE          (* CHANGED *)
    ...
```
the parser:
```
StmtM:  /* No unbalanced if-else */
  ...
  | SWITCH LPAR Expr RPAR LBRACE Cases RBRACE { Switch($3, $6) }
;

Cases:
                                  { [] }
  | CASE CSTINT COLON Block Cases { ($2, $4) :: ($5) }
;
```
```
%token ... SWITCH CASE  /* CHANGED */
```
and the compiler:
```fsharp
| Switch(e, cases) -> (* CHANGED *)
        let labend = newLabel()
        let rec aux cases =
            match cases with
            | [] -> []
            | (x, body) :: rest ->
                let labnext = newLabel()
                [DUP; CSTI x; EQ; IFZERO labnext]
                @ [INCSP -1]                        (* Remove result of expression from stack, as this is a statement *)
                @ cStmt body varEnv funEnv @ [GOTO labend]
                @ [Label labnext] @ aux rest 
        cExpr e varEnv funEnv @ aux cases @ [Label labend]
```
the following shows how to run the test case:
```shell
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs
```
```
open ParseAndComp;;
compileToFile (fromFile "tests/test2.c") "tests/test2.out";;
#q;;
```
```shell
java Machine.java
java Machine tests/test2.out
```
here is the test case:
```C
void main()
{
    int month;
    int y;
    int days;
    
    month = 2;
    y = 2020;
    
    switch (month) {
    case 1:
        { days = 31; }
    case 2:
        { days = 28; if (y%4==0) days = 29; }
    case 3:
        { days = 31; }
    }
    print days;
    println;
}
```
and this is the output, which matches the expected output:
```
29
```

# 9.1

# 9.3
```C#
private final Node dummy = new Node(-444, null);
private Node head = dummy, tail = dummy;
```
this was the culprit. dummy permenantly points to the sentinel node, so the garbage can never be collected by the garbage collector. we can fix this by removing the evil dummy node:
```C#
private Node head = new Node(-444, null), tail = head;
```
