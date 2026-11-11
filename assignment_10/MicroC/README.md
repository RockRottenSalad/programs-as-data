# 12.1
>[...] Define a new auxiliary function `addIFZERO lab3 C` [...] instead of doing `IFZERO labelse :: code` you must call the auxiliary to do the consing, as in addIFZERO labelse code. [...] A similar optimization can be made for `IFNZRO L3; GOTO L2; Label L3`.
```F#
let rec addIFZERO lab3 c : instr list = (* CHANGED *)
    match c with
    | GOTO lab2 :: Label lab3 :: c1 -> IFNZRO lab3 :: c1
    | _ -> IFZERO lab3 :: c

let rec addIFNZRO lab3 c : instr list = (* CHANGED *)
    match c with
    | GOTO lab2 :: Label lab3 :: c1 -> IFZERO lab2 :: c1
    | _ -> IFNZRO lab3 :: c
```
# 12.2
>Improve code generation in the continuation-based micro-C compiler so that a less-than comparison with constant arguments is compiled to its truth value.
> [...] Further improve the code generation so that all comparisons with constant arguments are compiled to the same code as true [...] or false.
```F#
let rec addCST i c =
    match (i, c) with
    ...
    | (n, CSTI m :: LT :: c1) -> if n < m then addCST 1 c1 else addCST 0 c1 (* CHANGED *)
    | (n, CSTI m :: EQ :: c1) -> if n = m then addCST 1 c1 else addCST 0 c1 (* CHANGED *)
```

# 12.3
>Extend the micro-C abstract syntax (file Absyn.fs) with conditional expressions Cond(e1, e2, e3) [...]

changes:

in `CLex.fsl`:
```F#
rule Token = parse
  ...
  | '?'             { QUESTION }
  | ':'             { COLON }
```

in `Absyn.fs`:
```F#
and expr =                                                         
  ...
  | Cond of expr * expr * expr       (* CHANGED                     *)
```

in `CPar.fsy`:
```F#
%right ASSIGN             /* lowest precedence */
%nonassoc PRINT
%left QUESTION COLON
%left SEQOR
...
```

```F#
ExprNotAccess:
  ...
  | Expr QUESTION Expr COLON Expr       { Cond($1, $3, $5)     } /* CHANGED */
;

```


for testing:
```F#
> compileToFile (fromFile "test/exA.c") "test/exA.out";; 
val it: Machine.instr list =
  [LDARGS 0; CALL (0, "L1"); STOP; Label "L1"; CSTI 1111; RET 0; Label "L2";
   CSTI 2222; RET 0]
```

# 12.4
```F#
ExprNotAccess:
  ...
  | Expr SEQAND Expr                    { Cond($1, $3, CstI 0)      } /* CHANGED */
  | Expr SEQOR  Expr                    { Cond($1, CstI 1, $3)      } /* CHANGED */
  | Expr QUESTION Expr COLON Expr       { Cond($1, $3, $5)     } /* CHANGED */
;
```


for testing:
```F#
> compileToFile (fromFile "CEx/ex13.c") "CEx/ex13.out";;
val it: Machine.instr list =
  [LDARGS 1; CALL (1, "L1"); STOP; Label "L1"; INCSP 1; GETBP; CSTI 1; ADD;
   CSTI 1889; STI; INCSP -1; GOTO "L3"; Label "L2"; GETBP; CSTI 1; ADD; GETBP;
   CSTI 1; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; GETBP; CSTI 1; ADD; LDI;
   CSTI 4; MOD; IFNZRO "L5"; GETBP; CSTI 1; ADD; LDI; CSTI 100; MOD;
   IFZERO "L6"; CSTI 1; GOTO "L4"; Label "L6"; GETBP; CSTI 1; ADD; LDI;
   CSTI 400; MOD; NOT; GOTO "L4"; Label "L5"; CSTI 0; Label "L4"; IFZERO "L3";
   GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; Label "L3"; GETBP; CSTI 1; ADD;
   LDI; GETBP; LDI; LT; IFNZRO "L2"; RET 1]
```