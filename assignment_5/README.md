
# Exercise 7.1
> Download `microc.zip` from the book homepage, unpack it to a folder MicroC, and build the micro-C interpreter as explained in `README.TXT` step (A).

This has been done.

> Run the `fromFile` psrser on the micro-C example in source file `ex1.c`. In your solution to the exercise, include the AST and indicate its part: declarations, statements, types and expressions.

When running `fromFile` with `ex01.c` as the argument we get the following:

(declarations, statements, types and expressions have been annotated with comments)

```fsharp
> open ParseAndRun;;
> fromFile "CEx/ex01.c";;
val it: Absyn.program =
  Prog
    [Fundec                (* Function declaration *)
       (None, "main", [(TypI, "n")],  (* Aforentioned func dec with name "main" and integer argument "n" *)
        Block
          [Stmt            (* While loop statement *)
             (While
                (Prim2 (">", Access (AccVar "n"), CstI 0), (* While loop guard expression *)
                 Block
                   [Stmt (Expr (Prim1 ("printi", Access (AccVar "n")))); (* Statement with single expression that prints the value of n *)
                    Stmt        (* Statement where we re-assign n to n - 1 *)
                      (Expr
                         (Assign
                            (AccVar "n",
                             Prim2 ("-", Access (AccVar "n"), CstI 1))))]));
           Stmt (Expr (Prim1 ("println", CstI 10)))])] (* print new line statement *)
```

# Exercise 7.2

> Write and run a few more micro-C programs to understand the use of arrays, pointer arithmetics, and parameter passing. Use the Micro-C implementaiton in `Interp.fs` and the associated lexer and parser to run your programs, as in Exercise 7.1

## Exercise 7.2 (i)
> Write a micro_C program containing a function void arrsum(int n, int arr[], int *sump) that computes and returns the sum of the first `n` elements of the given array `arr`. The reuslt must be returne dthrough the `sump` pointer. The program's main function must create an array holding the four numbers `7, 13, 9, 8`, call function `arrsum` on that array, and print the result using micro-C's non-standard `print` statement.


We wrote this in `MicroC/ex7_programs/ex7_2_i.c`.

```c
void main(int n) { 
    int sum;
    int arr[4];
    arr[0] = 7;
    arr[1] = 13;
    arr[2] = 9;
    arr[3] = 8;
    arrsum(n, arr, &sum);
    print sum;
}

void arrsum(int n, int arr[], int *sump) {
    int i; i = 0;
    int sum; sum = 0;
    while(i < n) {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
```

We then verify that the code is correct by running it. We correctly see "37"(right before `val`) as expected.
```fsharp
> run (fromFile "ex7_programs/ex7_2_i.c") [];;
37 val it: Interp.store =
  map
    [(0, 37); (1, 7); (2, 13); (3, 9); (4, 8); (5, 1); (6, 4); (7, 1); (8, 0);
     ...]
```


## Exercise 7.2 (ii)
> Write a micro-C program containing a function `void squares(int n, int arr[])` that given `n` and an array `arr` of length `n` or more fills `arr[i]` with `i*i` for `i = 0, ..., n - 1`.

We wrote this in `MicroC/ex7_programs/ex7_2_ii.c`.

```c
void main(int n) { 
    int sumofsquares;
    int arr[20];
    squares(n, arr);
    arrsum(n, arr, &sumofsquares);
    print sumofsquares;
}

void squares(int n, int arr[]) {
    int i; i = 0;
    while(i < n) {
        arr[i] = i * i;
        i = i + 1;
    }
}


void arrsum(int n, int arr[], int *sump) {
    int i;
    int sum;
    i = 0;
    sum = 0;
    while(i < n) {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
```

Now we verify that we get the correct result by trying `n=1`, `n=2`, `n=5`, `n=10` and `n=20`.

```fsharp
> run (fromFile "ex7_programs/ex7_2_ii.c") [1];;
0 val it: Interp.store =
  map
    [(0, 1); (1, 0); (2, 0); (3, -999); (4, -999); (5, -999); (6, -999);
     (7, -999); (8, -999); ...]

> run (fromFile "ex7_programs/ex7_2_ii.c") [2];;
1 val it: Interp.store =
  map
    [(0, 2); (1, 1); (2, 0); (3, 1); (4, -999); (5, -999); (6, -999);
     (7, -999); (8, -999); ...]

> run (fromFile "ex7_programs/ex7_2_ii.c") [5];;
30 val it: Interp.store =
  map
    [(0, 5); (1, 30); (2, 0); (3, 1); (4, 4); (5, 9); (6, 16); (7, -999);
     (8, -999); ...]

> run (fromFile "ex7_programs/ex7_2_ii.c") [10];;
285 val it: Interp.store =
  map
    [(0, 10); (1, 285); (2, 0); (3, 1); (4, 4); (5, 9); (6, 16); (7, 25);
     (8, 36); ...]

> run (fromFile "ex7_programs/ex7_2_ii.c") [20];;
2470 val it: Interp.store =
  map
    [(0, 20); (1, 2470); (2, 0); (3, 1); (4, 4); (5, 9); (6, 16); (7, 25);
     (8, 36); ...]
```

## Exercise 7.2 (iii)
> Write a micro-C program containing a function `void histogram(int n, int ns[], int max, int freq[])` which fills array `freq` the frequencies of the numbers in array `ns`. More precisely, when the function returns, element `freq[c]` must equal the number of times that value `c` appears among the first `n` elements of `arr`, for `0<=c<=max`. You can assume that all numbers in `ns` are between `0` and `max`, inclusive.


We wrote this in `MicroC/ex7_programs/ex7_2_iii.c`.

```c
void main(int n) { 
    int freq[4];
    freq[0] = 0;
    freq[1] = 0;
    freq[2] = 0;
    freq[3] = 0;

    int arr[7];
    arr[0] = 1;
    arr[1] = 2;
    arr[2] = 1;
    arr[3] = 1;
    arr[4] = 1;
    arr[5] = 2;
    arr[6] = 0;

    histogram(7, arr, 3, freq);
    printarr(4, freq);
}


void histogram(int n, int ns[], int max, int freq[]) {
    int i; i = 0;
    while(i < n) {
        freq[ns[i]] = freq[ns[i]] + 1;
        i = i + 1;
    }
}

void printarr(int len, int a[]) {
  int i; 
  i = 0; 
  while (i < len) { 
    print a[i]; 
    i = i + 1; 
  } 
}
```

Now we run it and verify we get what we expect.
Recall that our array is `[1, 2, 1, 1, 1, 2, 0]`,
so there should be 1 zero, 4 ones, 2 twos and 0 threes.

```fsharp
> run (fromFile "ex7_programs/ex7_2_iii.c") [];;
1 4 2 0 val it: Interp.store =
  map
    [(0, 1); (1, 4); (2, 2); (3, 0); (4, 0); (5, 1); (6, 2); (7, 1); (8, 1);
     ...]
```

Since we get `1 4 2 0`, our output is correct.

## Exercise 7.3

> Extend MicroC with a for-loop, permitting for instance `for(i = 0; i < 100; i=i+1) sum = sum+i`.

First we define `for` as a keyword inside `CLex.fsl`

```fsharp
let keyword s =
...
| "for"   -> FOR    (* CHANGED | Exercise 7.3 *)         
...
```

Afterwards, we add it as a token inside `CPar.fsy`.

```fsharp
%token CHAR ELSE IF INT NULL PRINT PRINTLN RETURN VOID WHILE FOR /* CHANGED | Exercise 7.3 */
```


And finally we add a rule for parsing for loops inside `CPar.fsy`.

```fsharp
StmtM:  /* No unbalanced if-else */
  ...
  | FOR LPAR Expr SEMI Expr SEMI Expr RPAR StmtM  { Block [ Stmt (Expr $3); Stmt (While($5, Block [Stmt $9; Stmt (Expr $7)])) ] } /* CHANGED | Exercise 7.3 */
;
```

```fsharp
StmtU:
  ...
  | FOR LPAR Expr SEMI Expr SEMI Expr RPAR StmtM  { Block [ Stmt (Expr $3); Stmt (While($5, Block [Stmt $9; Stmt (Expr $7)])) ] } /* CHANGED | Exercise 7.3 */
```

Now we re-generate our parser with `dotnet build parse.fsproj` and test that our for-loop implementation works.

We wrote this example C program inside `ex7_programs/ex7_3_test.c`
```c
void main(int n) { 
    int i;
    int sum;
    sum = 0;
    for(i = 1; i <= n; i = i + 1) {
        sum = sum + i;
    }
    print sum;
}
```

And when we run it, we get what we expect

```fsharp
> run (fromFile "ex7_programs/ex7_3_test.c") [1];;
1 val it: Interp.store = map [(0, 1); (1, 2); (2, 1)]

> run (fromFile "ex7_programs/ex7_3_test.c") [2];;
3 val it: Interp.store = map [(0, 2); (1, 3); (2, 3)]

> run (fromFile "ex7_programs/ex7_3_test.c") [3];;
6 val it: Interp.store = map [(0, 3); (1, 4); (2, 6)]

> run (fromFile "ex7_programs/ex7_3_test.c") [4];;
10 val it: Interp.store = map [(0, 4); (1, 5); (2, 10)]

> run (fromFile "ex7_programs/ex7_3_test.c") [5];;
15 val it: Interp.store = map [(0, 5); (1, 6); (2, 15)]

> run (fromFile "ex7_programs/ex7_3_test.c") [6];;
21 val it: Interp.store = map [(0, 6); (1, 7); (2, 21)]
```
