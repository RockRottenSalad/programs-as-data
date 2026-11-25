# Compiling and Loading the micro-Java Compiler

Chapter 14 presents the micro-Java language, which is inspired by Java
and has object-oriented language features such as dynamic method
dispatch, field hiding, method inheritance and method overriding.
These features present interesting new challenges in type checking
(described in this chapter) and in code generation (described in the
next chapter).  Micro-Java does not have static fields and methods,
interfaces, access modifiers, nested classes, generic types, generic
methods, packages and exceptions.  The micro-Java grammar and
semantics are kept as compatible with Java as possible.

Chapter 15 presents code generation for micro-Java (described in the
previous chapter), targeting an extended version of the abstract
machine used also for micro-C and micro-SML.  It is shown how virtual
instance method calls differ from non-virtual instance method calls
and how they are implemented via class descriptors and virtual method
tables in the run-time system.

## Building the micro-Java Command Line Compiler

The compiler has been tested on **.NET 8**, **.NET 9**, and **.NET
10**.

Choose the desired .NET version in the `microjc.fsproj` file by
changing:

```xml
<TargetFramework>net10.0</TargetFramework>
```

to one of:

- `net8.0`
- `net9.0`
- `net10.0`

Make `MicroJ` the current working directory.

To build the compiler, run:

```bash
dotnet build microjc.fsproj
```

or simply:

```bash
dotnet build
```

For example:

```bash
dotnet build
```

```bash
Restore complete (0.1s)
microjc net10.0 succeeded (0.1s) → bin/Debug/net10.0/microjc.dll

Build succeeded in 0.4s
```

The build process automatically runs the lexer and parser generators
whenever either `JLex.fsl` or `JPar.fsy` has changed.

The compiled compiler executable is placed in:

```bash
bin/Debug/net10.0/microjc.dll
```

## Compiling a micro-Java Program

To compile a micro-Java program, for example `JavaEx/ex01.java`, run:

```bash
dotnet run JavaEx/ex01.java
```

This compiles `ex01.java` from the `JavaEx` directory and generates
the output file:

```bash
JavaEx/ex01.out
```

Example:

```bash
dotnet run JavaEx/ex01.java
```

```bash
Micro-Java compiler v 1.00 of 2026-04-05
Compiling JavaEx/ex01.java to JavaEx/ex01.out.
```

## Compiler Options

The compiler supports the following command-line options:

| Option   | Description |
|------------|-------------|
| `-debug` | Outputs intermediate ASTs and other debugging information. |
| `-verbose` | Outputs intermediate program transformations. |
| `-genJavac` | Generates a Java SE 25 compliant source file in `JavaEx/Javac`, suitable for compilation with `javac`. |

Example:

```bash
dotnet run -debug -verbose -genJavac JavaEx/ex01.java
```

Options may be combined arbitrarily.

## The Micro Virtual Machine

The micro virtual machine, micro-VM, is located in the `MicroVM`
directory, where the file `README.md` explains how to build and use
it.

The result is an executable named:

- `microvm` (Unix/macOS)
- `microvm.exe` (Windows)

located in the `MicroVM` directory.

## A Complete Example

The following example demonstrates compiling and executing
`ex55.java`.

1. Change to the `MicroJ` directory.

2. Compile the program:

```bash
dotnet run JavaEx/ex55.java
```

```bash
Micro-Java compiler v 1.00 of 2026-04-05
Compiling JavaEx/ex55.java to JavaEx/ex55.out.
```
   
3. Run the compiled program using micro-VM:

```bash
../MicroVM/microvm JavaEx/ex55.out 5
```

```bash
1 3 5 2 4
1 4 2 5 3
2 4 1 3 5
2 5 3 1 4
3 1 4 2 5
3 5 2 4 1
4 1 3 5 2
4 2 5 3 1
5 2 4 1 3
5 3 1 4 2

10

Result value: #38361117416
Used 6 cpu milli-seconds
Number of GC: 0
```

## Micro-Java Example Programs

The micro-Java compiler comes with a test suite of more than **100
test programs** covering both static and dynamic semantics.

The test programs are located in the `JavaEx` directory.

- Programs named `exXX.java` compile and execute successfully with
  micro-Java. The example `ex55.java` above is one such program.

- Programs named `exFXX.java` demonstrate expected compile time
  errors, such as type errors.

For example, `exF34.java` demonstrates that redeclaring a local
variable is a type error. The error message refers back to the
corresponding type rule (see Chapter 14).

Example:

```bash
dotnet run JavaEx/exF34.java
```

```bash
Micro-Java compiler v 1.00 of 2026-04-05
Compiling JavaEx/exF34.java to JavaEx/exF34.out.

Type error on line 6, column 8:
  Variable i is already declared, (D-Var).
```

## Java SE 25 Compliance

Micro-Java has been designed so that translation into **Java SE 25**
compliant programs is generally straightforward. In most cases this is
possible automatically, with only a few semantic differences. For
example, `print` in micro-Java is an expression that leaves a result
on the stack.

The `-genJavac` compiler option generates a Java SE 25 compliant
version of the program in the `JavaEx/Javac` directory. The generated
file can then be compiled using `javac` and executed using `java`.

Using `ex55.java` as an example:

```bash
dotnet run -genJavac JavaEx/ex55.java
```

```bash
Micro-Java compiler v 1.00 of 2026-04-05
Compiling JavaEx/ex55.java to JavaEx/ex55.out.

GENJAVAC: Generated javac program in file JavaEx/Javac/ex55.java
```

From the `JavaEx/Javac` directory, compile and execute the generated
program:

```bash
javac ex55.java
```

```bash
java Main 5
```

```bash
1 3 5 2 4
1 4 2 5 3
2 4 1 3 5
2 5 3 1 4
3 1 4 2 5
3 5 2 4 1
4 1 3 5 2
4 2 5 3 1
5 2 4 1 3
5 3 1 4 2

10
```

## Micro-Java Test Suite

Micro-Java includes an automated test suite located in:

```bash
JavaEx/test.fsx
```

The test suite consists of three categories of programs:

1. **Programs that compile and run with both micro-Java and Java SE
25+**

   These verify **dynamic semantic compliance**.

2. **Programs that fail to compile with both micro-Java and Java SE
25+**

   These verify **static semantic compliance**.

3. **Programs that fail to compile with micro-Java but compile
successfully with Java SE 25+**

   An example is `exF40.java`, where micro-Java requires a `Main`
   class containing a `main` method. This is not a requirement in
   standard Java SE 25+.

Run the test suite from the `JavaEx` directory:

```bash
dotnet fsi test.fsx
```

```bash
Compiling file: ex01.java

  java SE 25 out: "42 43
"
  microJava out: "42 43
"
42 43

Compiling file: ex02.java

  java SE 25 out: "1 2 3
4 42 42 0 4
4 0 42
"
  microJava out: "1 2 3

...
Programs that succeed.
ex01.java: OK
ex02.java: OK
ex03.java: OK
ex04.java: OK
...
ex59.java: OK

Programs with type errors in both Micro-Java and Java.
exF01.java: OK
exF02.java: OK
exF03.java: OK
...
exF61.java: OK

Programs with type errors in Micro-Java.
exF40.java: OK
exF42.java: OK
...
```

The test script:

- compiles and runs all test programs,
- executes both the Java SE 25 and micro-Java versions where
  applicable,
- compares each execution with the expected output,
- summarizes the results at the end,
- works across multiple platforms.

> **Note:** The test suite runs relatively slow because it launches
    external system processes to compile and execute the programs.

