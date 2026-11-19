// Script to compile and run example programs with micro-Java and
// javac SE 25 compilers and compare output

// Script expects started from the MicroJ/JavaEx directory, e.g.:
//   dotnet fsi test.fsx

#load "cmd.fsx"
open Cmd
open System.IO
open System

let mutable verbose = true

let println s = if verbose then printfn "%s" s else ()

// Make sure current directory is MicroJ required to use dotnet run.
let testdir = "JavaEx"
let javacdir = "Javac"

// microVM executable depends on OS
let microvm =
  if System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) then
    "microvm.exe -silent"
  else
    "microvm -silent" 

let setCurDir() =
  System.Environment.CurrentDirectory <- "..";

setCurDir()

if Path.GetFileName(System.Environment.CurrentDirectory) <> "MicroJ"
  then failwith "test.fsx: Not in MicroJ folder - need to load test.fsx from JavaEx folder"

// Delete *.out files
let delOut() = runCmd ("rm -f " + Path.Combine(testdir,"*.out"))
delOut();

// Compile with micro-Java without generating a Java SE 25 file.
let microJ f = runCmd ("dotnet run " + Path.Combine (testdir,f))

// Compile with micro-Java and generate a Java SE 25 file.
let genJavac f = runCmd ("dotnet run -genJavac " + Path.Combine (testdir,f))

// Compile with Java SE 25 javac compiler.
let javac f = runCmd("javac " + Path.Combine [|testdir;javacdir;f|])

// Run Java SE 25 program. c is the class holding main method, e.g., Main.
let java c arg =
  runCmd("java -cp " + Path.Combine [|testdir;javacdir|] + " " + c + " " + arg)

// Run micro-Java program, that is *.out file, using the microVM machine.
let microJava (f:string) arg =
  let stem = if f.EndsWith(".java") then f.Substring(0,f.Length-5) else f
  let target = stem + ".out"
  runCmd(Path.Combine ([|"..";"MicroVM";microvm|]) + " " + Path.Combine(testdir,target) + " " + arg)

// Run a program f with arguments a on both Java SE 25 and micro-Java.
// Assumes programs are already compiled.
// Compares output.
let runOne f a =
  let c = "Main"  // Class containing main method in micro--Java.
  let (outSE25,errSE25,codeSE25) = java c a
  println ("  java SE 25 out: \"" + outSE25 + "\"")
  
  let (out,err,code) = microJava f a
  println ("  micro-Java out: \"" + out + "\"")
  let ok = if (outSE25.Contains("error") || out.Contains("error") || outSE25 <> out) then "ERROR" else "OK"
  if err <> ""
    then println ("    Error " + err)
    else println out
  (f,ok,out,err,code)

// Compile as run program f with given arguments cs on both Java SE 25 and micro-Java.
let test (f,cs) =
  printfn "Compiling file: %s" f;
  let (out, err, code) = genJavac f
  let (outSE25, errSE25, codeSE25) = javac f
  if err <> "" || errSE25 <> ""
    then println ("with error " + err + " " + errSE25)
    else println "";
  List.map (runOne f) cs


// Tests that succeed with both micro-Java and Java SE 25.
let tests = [("ex01.java",[""]);
             ("ex02.java",[""]);
             ("ex03.java",[""]);             
             ("ex04.java",[""]);
             ("ex05.java",[""]);
             ("ex06.java",[""]);
             ("ex07.java",[""]);
             ("ex08.java",[""]);
             ("ex09.java",["true";"false"]);
             ("ex10.java",["42"]);
             ("ex11.java",["23"]);
             ("ex12.java",["2026 2 14"]);
             ("ex13.java",[""]);
             ("ex14.java",[""]);
             ("ex15.java",["0";"1";"10"]);
             ("ex16.java",[""]);
             ("ex17.java",[""]);
             ("ex18.java",[""]);
             ("ex19.java",[""]);
             ("ex20.java",[""]);
             ("ex21.java",[""]);
             ("ex22.java",[""]);
             ("ex23.java",[""]);
             ("ex24.java",[""]);
             ("ex25.java",[""]);
             ("ex26.java",[""]);
             ("ex27.java",[""]);
             ("ex28.java",[""]);
             ("ex29.java",[""]);
             ("ex30.java",[""]);
             ("ex31.java",[""]);
             //("ex32.java",[""]); Will not be same address and hence output differs.
             ("ex33.java",[""]);
             ("ex34.java",["0";"1";"10";"25"]);
             ("ex35.java",["0";"1";"10";"25"]);
             ("ex36.java",[""]);
             ("ex37.java",["0";"1";"10";"25"]);
             ("ex38.java",["1900";"2000";"2010";"2024";"2025"]);
             ("ex39.java",["0";"1";"9";"25";"36"]);
             ("ex40.java",["0"; "1"; "40"]);
             ("ex41.java",["0"; "-1"; "1"]);
             ("ex42.java",[""]);
             ("ex43.java",["0 0"; "1 0"; "0 1"; "1 1"]);
             ("ex44.java",["0 0"; "1 0"; "0 1"; "1 1"]);
             ("ex45.java",["1000"; "1996"; "2000"; "2029"]);
             ("ex46.java",[""]);
             ("ex47.java",[""]);
             ("ex48.java",["32"]);
             ("ex48.java",["1000"]);
             ("ex49.java",["1000"]);
             ("ex50.java",["1000";"2000"]);
             ("ex51.java",[""]);             
             ("ex52.java",[""]);
             ("ex53.java",[""]);
             //("ex54.java",[""]); Class addresses differs and can't be compared.
             ("ex55.java",["5"; "7"]);
             ("ex56.java",["1"; "10"; "20"]);
             ("ex57.java",["1"; "10"]);
             ("ex58.java",[""]);
             ("ex59.java",[""]);
             ("ex63.java",[""]);
             //("ex64.java",[""]); Class addresses differs and can't be compared.
             ("ex70.java",["3000"]);  // 3000 does not depend on GC.
             ("ex71.java",["3000"]);  // 3000 does not run out of memory.
             ("ex73.java",["300"]);   // 300 does not run out of memory.
             ("ex74.java",[""]);
             ("ex75.java",["3000"]);  // 3000 does not depend on GC.
             ("ex77.java",[""])]
                    
// Compile and run tests with both Java SE 25 and micro-Java.  To use
// Array.Parallel.map requires that individual compilation directories
// are used for each test case.
let javaRes = List.map test tests

// Compile and expect type errors on both Java SE 25 and micro-Java.
let testTypErrMicroJAndJavac f =
  printfn "\nCompiling program %s with micro-Java" f
  let (out,err,code) = genJavac f
  let okmj = out.Contains("error")
  if err <> ""
    then println ("    Error " + err)
    else println out
  printfn "\nCompiling program %s with javac" f
  let (out,err,code) = javac f
  let okjavac = err.Contains("error")
  if err <> ""
    then println ("    Error " + err)
    else println out
  let ok = if okmj && okjavac then "OK" else "ERROR"
  (f,ok)

let typErrTests = ["exF01.java"; "exF02.java"; "exF03.java"; "exF04.java"; "exF05.java";
                   "exF06.java"; "exF07.java"; "exF08.java"; "exF09.java"; "exF10.java";
                   "exF11.java"; "exF12.java"; "exF13.java"; "exF14.java"; "exF15.java";
                   "exF16.java"; "exF17.java"; "exF18.java"; "exF19.java"; "exF20.java";
                   "exF21.java"; "exF22.java"; "exF23.java"; "exF24.java"; "exF25.java";
                   "exF26.java"; "exF27.java"; "exF28.java"; "exF29.java"; "exF30.java";
                   "exF31.java"; "exF32.java"; "exF33.java"; "exF34.java"; "exF35.java";
                   "exF36.java"; "exF37.java"; "exF38.java"; "exF39.java"; 
                   "exF44.java"; "exF45.java"; "exF47.java"; "exF61.java"]
let testAllTypErrs = List.map testTypErrMicroJAndJavac typErrTests

// Compile and expect type errors on micro-Java.
// Some do not parse and therefore no Java code emitted.
let testTypErrMicroJOnly f =
  printfn "\nCompiling program %s with micro-Java" f
  let (out,err,code) = microJ f
  let okmj = out.Contains("error")
  if err <> ""
    then println ("    Error " + err)
    else println out
  let ok = if okmj then "OK" else "ERROR"
  (f,ok)
let typErrTestsMicroJOnly = ["exF40.java"; "exF41.java"; "exF42.java"; "exF43.java";
                             "exF59.java"; "exF60.java"; "exF62.java"; "exF63.java"]
let testAllTypErrsMicroJOnly = List.map testTypErrMicroJOnly typErrTestsMicroJOnly

verbose <- true
println "Programs that succeed."
List.map (fun xs -> List.map (fun (f,ok,_,_,_) -> println (f + ": " + ok)) xs) javaRes
println "Programs with type errors in both micro-Java and Java."
List.map (fun (f,ok) -> println (f + ": " + ok)) testAllTypErrs
println "Programs with type errors in micro-Java."
List.map (fun (f,ok) -> println (f + ": " + ok)) testAllTypErrsMicroJOnly




