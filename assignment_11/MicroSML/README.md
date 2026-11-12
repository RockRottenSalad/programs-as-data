# Compiling and Loading the micro-SML Compiler

Chapter 13 presents the micro-SML language, a subset of Core Standard
ML and an extension of micro-ML.  This rounds off the topic of
functional languages by putting together many of the previously
discussed topics of higher-order functions, polymorphic type
inference, heap allocation, garbage collection, continuations and
local optimization techniques previously applied to micro-C.

## Building the micro-SML command line compiler

The compiler has been tested on **.NET 8**, **.NET 9** and .NET 10**.

Choose the desired .NET version in the `microsmlc.fsprog` file by
changing:

```xml
<TargetFramework>net10.0</TargetFramework>
```

to one of

- `net8.0`
- `net9.0`
- `net10.0`

Make `MicroSML` current directory.

To build the compiler, run:

```bash
dotnet build microsmlc.fsproj
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
Restore complete (0,1s)
microsmlc net10.0 succeeded (0,1s) → bin/Debug/net10.0/microsmlc.dll

Build succeeded in 0,4s
```

The build process automatically runs the lexer and parser generators
whenever either `FunLex.fsl` or `FunPar.fsy` has changed.

The compiled compiler executable is placed in:

```bash
bin/Debug/net10.0/microsmlc.dll
```

## Compiling a micro-SML program

To compile a micro-SML program, for example `SmlEx/ex01.sml`, run:

```bash
dotnet run SmlEx/ex01.sml
```

This compiles `ex01.sml` from the `SmlEx` directory and generates the
output file:

```bash
SmlEx/ex01.out
```

Example:

```bash
dotnet run SmlEx/ex01.sml
```

```bash
Micro-SML compiler v 2.0 of 2026-05-30
Compiling SmlEx/ex01.sml to SmlEx/ex01.out.
```

## Compiler options

The compiler supports below options:

| Option   | Description |
|------------|-------------|
| `-debug` | outputs intermediate AST and other debug information on terminal |
| `-verbose` | outputs intermediate program transformations on terminal |
| `-eval` | Interpretates the program and outputs result on terminal. |
| `-alpha` | Performs alpha conversion, left as exercise. |
| `-opt` | Performs local peephole optimizations. |

Example:

```bash
dotnet run -eval -verbose SmlEx/ex01.sml
```

Options can be combined arbitrarily.

## The Micro virtual machine

The micro virtual machine, micro-VM, is located in the `MicroVM`
directory, where the file `README.md` explains how to build and use
it.

The result is an executable named:

- `microvm` (Unix/macOS)
- `microvm.exe` (Windows)

located in the `MicroVM` directory.

## A complete example

The following example demonstrates compiling and executing
`queens.sml`.

1. Change to the `MicroSML`directory.

2. Compile the program

```bash
dotnet run SmlEx/queens.sml
```

```bash
Micro-SML compiler v 2.0 of 2026-05-30
Compiling SmlEx/queens.sml to SmlEx/queens.out.
```

3. Run the compiled program using micro-VM:

```bash
../MicroVM/microvm SmlEx/queens.out
```

```bash
[[[6,5] ,[5,3] ,[4,1] ,[3,6] ,[2,4] ,[1,2] ] ,
  [[5,4] ,[4,2] ,[3,5] ,[2,3] ,[1,1] ] ,
  [[4,3] ,[3,1] ,[2,4] ,[1,2] ] ,[] ,[] ,[[1,1] ] ]

Result value: #33629982712
Used 1 cpu milli-seconds
Number of GC: 0
```

## Micro-SML example programs

The micro-SML compiler comes with a test suite of **50 test programs**
covering both static and dynamic semantics.

The test programs are in the `SmlEx` directory.

- Programs named `exXX.sml` compiles and run with micro-SML. The
program `queens.sml` used above is an example.

- Programs named `exFXX.sml` demonstrate expected compile time errors,
  such as type errors.

For example, `exF02.sml` demonstrates a problem unifying two types
that cannot be unified.

Example:

```bash
dotnet run SmlEx/exF02.sml
```

```bash
Micro-SML compiler v 2.0 of 2026-05-30
Compiling SmlEx/exF02.sml to SmlEx/exF02.out.

Type error: bool and int
```

## Micro-SML test suite

Micro-SML includes an automated test suite located in:

```bash
SmlEx/test.fsx
```

The test suite consists of three categories of test programs:

1. Programs that compile and run with Micro-SML, `exXX.sml`.

These verify static and dynamic semantic compliance.

2. Same as 1), but focus on generative exceptions, `exnXX.sml`.

3. Programs that fail to compile with micro-SML due to type errors.

  These verify the static semantics.

Run the test suite from the `SmlEx` directory:

```bash
dotnet fsi test.fsx
```

```bash
Setting current directory to MicroSML
Delete all *.out files in SmlEx directory.
Compiling file with options -eval: ex01.sml
outMatch: [5]

Compiling file with options : ex01.sml

micro-SML out: [5]
 5

...

Result of test.
ex01.sml(Eval): OK
ex01.sml(Comp): OK
ex01.sml(Comp+Opt): OK
ex02.sml(Eval): OK
ex02.sml(Comp): OK
ex02.sml(Comp+Opt): OK
ex03.sml(Eval): OK
...
```

The test script

- compiles and runs all test programs.
- compiles with different combinations of compiler options. See
`test.fsx` for the combinations used.
- compares each execution with the expected output.
- summarizes the results at the end.
- works across multiple platforms.

> **Note** The test suite runs relatively slow because it launches
external system processes to compile and execute the programs.


