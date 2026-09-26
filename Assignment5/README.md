# Exercise 6.5
> (1) Use the type inference on the micro-ML programs shown below, and report what
type the program has. Some of the type inferences will fail because the programs are
not typable in micro-ML; in those cases, explain why the program is not typable:

Below we go through all the examples that fail.

The first failure comes from:
```
let f g = g g in f end
```
And the reason this fails is because g can't be assigned a type. So as per type inference we start of by giving `g` some typevariable `a`. 
We then see that `g` is applied to itself. This must mean that `a = b -> c`, since `g` is a function. However, `g` is also the argument, so it must be the case that
`a = b`. This means that `a = a -> c`, which is not typable, since we can't unify thesee types, because `a` occures in both branches. If we tried to unify it the following would happen:
```
a = a -> c =>
a = (a -> c) -> c =>
a = ((a -> c) -> c) -> c =>
....
```
So it would never stop.

The second example that fails is:
```
let f x =
let g y = if true then y else x
in g false end
in f 42 end
```
The reson type inference fails is because `y` and `x` depend on eachother. Inside `g` the variable `y` forces `x` to have the type bool, which means `f`
is inferred to have type `f: bool -> bool`, but `f` is called with an int, so type inference fails.

> (2) (2) Write micro-ML programs for which the micro-ML type inference report the
following types:

Below is a program which type is inferred to be `bool -> bool`:
```fsharp
let f x =
    if x then true else false
in f end
// This typechecks to: (bool -> bool)
```

Below is a program which type is inferred to be `int -> int`:
```fsharp
let f x =
    if x = 1 then 0 else 1
in f end
// This typechecks to: (int -> int)
```
Below is a program which type is inferred to be `int -> int -> int`:
```fsharp
let f x =
    let g y = x + y
    in g end
in f end
// This typechecks to: (int -> (int -> int))
```
Below is a program which type is inferred to be `'a -> 'b -> 'a`:
```fsharp
let f x =
    let g y = x in g end
in f end
// This typechecks to: ('h -> ('g -> 'h))
```
Below is a program which type is inferred to be `('a -> 'b) -> ('b -> c) -> ('a -> 'c)`:
```fsharp
let f x =
    let g y =
        let z a = y (x a)
        in z end
    in g end
in f end
// This typechecks to: (('l -> 'k) -> (('k -> 'm) -> ('l -> 'm)))
```

Below is a program which type is inferred to be `'a -> 'b`:
```fsharp
let f x = f x in f end
// This typechecks to: ('a -> 'b)
```

Below is a program which type is inferred to be `'a`:
```fsharp
let f x = f x in f 1 end
// This typechecks to: 'f
```

# Exercise 7.1
> Run the fromFile parser on the micro-C example in source file ex1.c. In
your solution to the exercise, include the abstract syntax tree and indicate its parts:
declarations, statements, types and expressions.

Below the parts of the abstract syntax is shown:
```
Prog							
  [Fundec 								 # Function declaration
     (None, "main", [(TypI, "n")],
      Block
        [Stmt								 # Statement
           (While
              (Prim2 (">", Access (AccVar "n"), CstI 0),		 # Expression and 
               Block
                 [Stmt (Expr (Prim1 ("printi", Access (AccVar "n")))); # Statement
                  Stmt						 # Statement due to ;
                    (Expr						 # Expression				
                       (Assign
                          (AccVar "n",
                           Prim2 ("-", Access (AccVar "n"), CstI 1))))]));
         Stmt (Expr (Prim1 ("println", CstI 10)))])]			 # Statement		
```

# Exercise 7.2
> Write and run a few more micro-C programs to understand the use of
arrays, pointer arithmetics, and parameter passing.
> 
> (i) Write a micro-C program containing a function `void arrsum(int n, int
arr[], int *sump)` that computes and returns the sum of the first `n` elements
of the given array `arr`. (...)

The following defines the main function and sets up the array with the desired values etc.
```cs
void main()
{
    int a[4];
    a[0] = 7;
    a[1] = 13;
    a[2] = 9;
    a[3] = 8;
    
    int sum;
    arrsum(4, a, &sum);
    print sum;
    println;
    
}
```
The algorithm is very simple. We initialize a sum variable and a counter `i`. While the counter (the current element) is less than the desired amount of elements `n`, we add the value of the current element `arr[i]` to the `sum`. Once we are done, we simply dereference the pointer to `sump` and write the value of the sum.
```cs
void arrsum(int n, int  arr[], int *sump)
{
    int sum;
    int i;
    i = 0;
    sum = 0;
    
    while (i < n)
    {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
```
This can also be found in the `Peak/i.c` file. Now running the program:
```
> run (fromFile "peak/i.c") [];;
37 
```

> (ii) Write a micro-C program containing a function `void squares(int n,
int arr[])` that, given `n` and an array arr of length `n` or more fills `arr[i]`
with `i*i` for `i = 0, . . . , n − 1`.

Declaring the main function is pretty straight forward once again:
```cs
void main()
{
    int a[10];
    int sum;
    squares(10, a);
    arrsum(10, a, &sum);
    
    print sum;
    println;
}
```
Now for computing the squares `i*i` for `0, 1, ..., n - 1`, we simply have a counter `i`, and while `i < n`, we write `i * i` to `arr[i]`:
```csharp
void squares(int n, int arr[])
{
    int i;
    i = 0;
    while (i < n)
    {
        arr[i] = i * i;
        i = i + 1;
    }
}
```
Running this in `fsi` we get:
```
> run (fromFile "peak/ii.c") [];;
285 
```
> Write a micro-C program containing a function `void histogram(int n,
int ns[], int max, int freq[])` which fills array freq the frequencies
of the numbers in array ns.

The code is pretty self explanatory:
```csharp
void main()
{
    int ns[7];
    ns[0] = 1;
    ns[1] = 2;
    ns[2] = 1;
    ns[3] = 1;
    ns[4] = 1;
    ns[5] = 2;
    ns[6] = 0;
    
    int freq[4];
    
    histogram(7, ns, 3, freq);
    
    printarr(freq, 4);
}

void histogram(int n, int ns[], int max, int freq[])
{
    int count;
    
    int c;
    c = 0;
    while (c < max + 1)
    {
        count = 0;
        int i;
        i = 0;
        
        while (i < n)
        {
            if (ns[i] == c)
            {
                count = count  +1;
            }
            i = i + 1;
        }
        
        freq[c] = count;
        c = c + 1;
    }
}

void printarr(int arr[], int n)
{
    int i;
    i = 0;
    while (i < n)
    {
        print arr[i];
        i = i + 1;
    }
    println;
}

```

# Exercise 7.3
> Extend MicroC with a for-loop, permitting for instance
> ```csharp
> for (i=0; i<100; i=i+1)
>     sum = sum+i;
> ```

The first step to doing this is extending the lexer to accept the token "for":
```
let keyword s =
    match s with
    ...       
    | "for"     -> FOR
    | _         -> NAME s
```
And then inside the parser making sure to specify its a token:
```
%token ... FOR
```
The general way we could parse a for loop of the type
```
for (e1; e2; e3) {
    stmnt
}
```
Is to first read the key word `FOR` then a pair of paranteheses, and inside the parantehese read three expressions of type `Expr` seperated by `;` and then a statement (the body of the loop), i.e.
```
FOR LPAR Expr SEMI Expr SEMI Expr RPAR StmtM
```
As the exercise mentioned a clever way of encoding a for loop is using `Block`, `While` and `Expr` constructors from the abstract syntax, i.e. a for loop can be written as:
```
{
    e1;
    while (e2) {
        stmt
        e3;
    }
}
```
We can translate this into abstract syntax and finish our extra production rule like so:
```
StmtM:  /* No unbalanced if-else */
  ...
  | FOR LPAR Expr SEMI Expr SEMI Expr RPAR StmtM  { Block([Stmt(Expr($3)); Stmt (While($5, Block([Stmt($9); Stmt(Expr($7))])))]) } (* Changed *)
;
```

Each of the rewritten programs can be found within `Peak/i.c`, `Peak/ii.c` and `Peak/iii.c`. The functions have their original names, but with a `2` added to the end. Please note we also implemented postfix increment and decrement form `7.4` and `7.5` so the rewritten exmaples use that. Mentally, `++a = a + 1`. We also implemented support for `+=`, `-=`, `*=`, `/=` and `%=`, i.e. compound assignments as per exercise 7.6.