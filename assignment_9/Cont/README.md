# Compiling and loading continuation-based evaluators

Chapter 11 introduces the concept of continuation, which helps
understand such notions as tail call, exceptions and exception
handling, execution stack, and backtracking.  A continuation is an
explicit representation of the rest of the computation.  Usually this
is implicit in a program: after executing one statement, the
computation will continue with the next statement; when returning from
a method, the computation will continue where the method was called;
and so on.  Making the continuation explicit has the advantage that we
can ignore it (and so model abnormal termination), and that we can
have more than one continuation (and so model exception handling and
backtracking).


## A. Two continuation-based interpreters for functional language

Loading two continuation-based interpreters for a functional language
with exceptions:

```bash
dotnet fsi Contfun.fs
```

```fsharp
open Contfun;;
eval1 ex1 [];; 
```

```fsharp
eval1 ex2 [("n", Int 10)];;
#q;;
```

## B. Two continuation-based interpreters for an imperative language

Loading two continuation-based interpreters for an imperative language
with exceptions:

```bash
dotnet fsi Contimp.fs
```

```fsharp
open Contimp;;
run1 ex1;;
```

```fsharp
run1 ex2;;
```

```fsharp
run2 ex3;;
#q;;
```

## C. A continuation-based interpreter for micro-Icon

Loading a continuation-based interpreter for micro-Icon, a language in
which an expression can have multiple results:

```bash
dotnet fsi Icon.fs
```

```fsharp
open Icon;;
run ex1;;
```

```fsharp
run ex2;;
```

```fsharp
run ex3and;;
```

```fsharp
run ex3or;;
#q;;
```

## D. Java implementation of factorial

Compile and run a Java implementation of factorial in
continuation-passing style:

```bash
cd Factorial/

javac Factorial.java
```

```bash
java Factorial 10
```

```bash
javac Factorial2.java
```

```bash
java Factorial2 10
```

## E. C# implementation of factorial 

Compile and run a C# implementation of factorial in
continuation-passing style:

```bash
cd Factorial/

dotnet build Factorial.csproj
```

```bash
dotnet run 10
```

## F. Example illustrating longjmp

Compile and run example illustrating longjmp and setjmp in C (under
Linux and MacOS):

We recommend using `clang` as compiler. See [Platform
Dependencies](../README.md) on how to install across platforms.

```bash
clang testlongjmp.c -o testlongjmp
```

```bash
./testlongjmp 10
```

```bash
./testlongjmp 11
```
