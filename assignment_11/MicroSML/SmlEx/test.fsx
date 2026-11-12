// Script to compile and run example programs with micro-SML.

// Script expects started from the MicroSML/SmlEx directory, i.e.:
//   dotnet fsi test.fsx

#load "cmd.fsx"
open Cmd
open System.IO
open System

// Alpha conversion is an exercise and not considered here.
type testType =
    EVAL                 // Evaluate and compare without Result value: ...
  | COMP                 // Compile and compare without Reuslt value: ...
  | COMP_OPT             // Compile with optimizations and compare without Result value: ...
  | EVAL_NON_SILENT      // Include Result value: ... in comparison.
  | COMP_NON_SILENT      // Include Result value: ... in comparison.
  | COMP_OPT_NON_SILENT  // Include Result value: ... in comparison.
  | TYPE_ERROR           // Compile and verify type error.
  | PARSE_ERROR          // Compile and verify parse error.  

let ppTestType = function
    EVAL     -> "Eval"
  | COMP     -> "Comp"
  | COMP_OPT -> "Comp+Opt"
  | EVAL_NON_SILENT -> "Eval+NonSilent"  
  | COMP_NON_SILENT -> "Comp+NonSilent"
  | COMP_OPT_NON_SILENT -> "Comp+Opt+NonSilent"
  | TYPE_ERROR -> "TypeError"
  | PARSE_ERROR -> "ParseError"  
  
// Combinations of test types used often
let allTT = [EVAL;COMP;COMP_OPT]
let allNonSilentTT = [EVAL_NON_SILENT;COMP_NON_SILENT;COMP_OPT_NON_SILENT]
let allCompTT = [COMP;COMP_OPT]
let evalTT = [EVAL]
let typeErrTT = [TYPE_ERROR]
let parseErrTT = [PARSE_ERROR]

let nl = System.Environment.NewLine
let mutable verbose = true

let println s = if verbose then printfn "%s" s else ()

// Make sure current directory is MicroSML required to use dotnet run.
let testdir = "SmlEx"

// microVM executable depends on OS
let microvm =
  if System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) then
    "microvm.exe"
  else
    "microvm" 

let setCurDir() =
  System.Environment.CurrentDirectory <- ".."

println("Setting current directory to MicroSML")
setCurDir()

if Path.GetFileName(System.Environment.CurrentDirectory) <> "MicroSML"
  then failwith "test.fsx: Not in MicroSML folder - need to load test.fsx from SmlEx folder"

// Delete *.out files
let delOut() = runCmd ("rm -f " + Path.Combine(testdir,"*.out"))
println("Delete all *.out files in SmlEx directory.")
delOut()

// Compile with micro-SML
let compMicroSML opt f = runCmd ("dotnet run " + opt + " " + Path.Combine (testdir,f))

// Run micro-SML program, that is *.out file, using the microVM machine.
let runMicroSML opt (f:string) arg =
  let stem = if f.EndsWith(".sml") then f.Substring(0,f.Length-4) else f
  let target = stem + ".out"
  runCmd(Path.Combine ([|"..";"MicroVM";microvm|]) + " " + opt + " " + Path.Combine(testdir,target) + " " + arg)

// Assumes programs are already compiled.
// Compares output.
let runOne opt f (expected:string) =
  let (out,err,code) = runMicroSML opt f ""
  let outMatch = 
    if opt = "-silent"
      then out
      else let postfixIdx = out.IndexOf("Used") - 2
           out[..postfixIdx]
  println ("  micro-SML out: [" + outMatch + "]")
  let ok = if outMatch.Trim() <> expected.Trim() then "ERROR" else "OK"
  if err <> ""
    then println ("    Error " + err)
    else println out
  (f,ok,out,err,code)

// Compile and run program f with given arguments cs.
let test vmOpt opt (f,expected) =
  printfn "Compiling file with options %s: %s" opt f;
  let (out, err, code) = compMicroSML opt f
  if err <> "" 
    then println ("with error " + err)
    else println "";
  runOne vmOpt f expected

// Compile and evaluate program f with given arguments cs.
let testEval nonSilent_p opt (f,expected:string) =
  printfn "Compiling file with options %s: %s" opt f;
  let (out, err, code) = compMicroSML opt f
  let (prefixIdx,postfixIdx) =
    (out.IndexOf("Program") + 8,
     if nonSilent_p
       then out.IndexOf("Used") - 2
       else out.IndexOf("Result") - 2)
  let outMatch = out[prefixIdx..postfixIdx]
  println("outMatch: [" + outMatch + "]");
  if err <> "" 
    then println ("with error " + err)
    else println "";
  let ok = if outMatch.Trim() <> expected.Trim() then "ERROR" else "OK"    
  (f,ok,out,err,code)

let testTypeError f =
  printfn "Compiling file %s" f;
  let (out, err, code) = compMicroSML "" f
  println out
  let ok = if out.IndexOf "Type error:" <> -1 then "OK" else "ERROR"
  (f,ok,out,err,code)

let testParseError f =
  printfn "Compiling file %s" f;
  let (out, err, code) = compMicroSML "" f
  println out
  let ok = if out.IndexOf "parse error" <> -1 then "OK" else "ERROR"
  (f,ok,out,err,code)

let doTest f expected testType =
  let res = match testType with
              EVAL -> testEval false "-eval" (f,expected)
            | COMP -> test "-silent" "" (f,expected)
            | COMP_OPT -> test "-silent" "-opt" (f,expected)
            | EVAL_NON_SILENT -> testEval true "-eval" (f,expected) 
            | COMP_NON_SILENT -> test "" "" (f,expected)
            | COMP_OPT_NON_SILENT -> test "" "-opt" (f,expected)
            | TYPE_ERROR -> testTypeError f
            | PARSE_ERROR -> testParseError f
  (testType,res)

let doOneTestAllTestTypes (f,expected,testTypes) =
  List.map (doTest f expected) testTypes

let doAllTestsAllTestTypes tests =
  List.map doOneTestAllTestTypes tests

// Tests that succeeds
let tests =
  [("ex01.sml","5",allTT);
   ("ex02.sml","[0,1,2,3,4,5,6,7,8,9,10] [1,2,3,4,5,6,7,8,9,10,11]",allTT);
   ("ex03.sml","42",allTT);
   ("ex04.sml","10 2 25",allTT);
   ("ex05.sml","26",allTT);
   ("ex06.sml","42",allTT);
   ("ex07.sml","",allTT);
   ("ex08.sml","6",allTT);
   ("ex09.sml","4",allTT);
   ("ex10.sml","",allTT);
   ("ex11.sml","",evalTT); // Requires alpha conversion fixed for comp to work, see exercises.
   ("ex12.sml","",allTT);
   ("ex13.sml","true 32 [] 1 false Fn@annFunc1(0)  Fn@output(0)",evalTT);
   ("ex13.sml","true 32 [] 1 false Fn@36(0) Fn@28(0)",allCompTT);   // Code address only changes if code generator changes.
   ("ex14.sml","4 9 5 4 4 4",allTT);
   ("ex17.sml","43",allTT);         
   ("ex18.sml","42",allTT);      
   ("ex19.sml","85 -1",allTT);   
   ("exn01.sml","true true true true true 1 3 4 true 42 42 true " + nl + "Result value: 1",allNonSilentTT);
   ("exn02.sml","42 4242 " + nl + "Result value: Uncaught exception 2",allNonSilentTT);
   ("exn03.sml","42 4242 " + nl + "Result value: Uncaught exception 2",allNonSilentTT);
   ("exn04.sml","1 1 " + nl + "Result value: Uncaught exception 2",allNonSilentTT);
   ("exn05.sml","",allNonSilentTT);
   ("exn06.sml",nl + "Result value: Uncaught exception 2",allNonSilentTT);
   ("exn07.sml",nl + "Result value: Uncaught exception 1",allNonSilentTT);
   ("exn08.sml",nl + "Result value: Uncaught exception 1",allNonSilentTT);      
   ("list.sml","true true true true true true true true true true",allTT);
   ("queens.sml","[[[6,5] ,[5,3] ,[4,1] ,[3,6] ,[2,4] ,[1,2] ] ,[[5,4] ,[4,2] ,[3,5] ,[2,3] ,[1,1] ] ,[[4,3] ,[3,1] ,[2,4] ,[1,2] ] ,[] ,[] ,[[1,1] ] ]",allCompTT); // Stack overflow on evaluation
   ("test01.sml","true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true true",allTT);
   ("test02.sml","true true true true true true true",allTT);
   ("exF01.sml","",typeErrTT);
   ("exF02.sml","",typeErrTT);
   ("exF03.sml","",typeErrTT);
   ("exF04.sml","",typeErrTT);
   ("exF05.sml","",typeErrTT);
   ("exF06.sml","",typeErrTT);
   ("exF07.sml","",typeErrTT);
   ("exF08.sml","",typeErrTT);
   ("exF09.sml","",typeErrTT);
   ("exF10.sml","",typeErrTT);
   ("exF11.sml","",parseErrTT);
   ("exF12.sml","",typeErrTT);
   ("exF13.sml","",typeErrTT);
   ("exF14.sml","",typeErrTT);
   ("exF15.sml","",typeErrTT);
   ("exF16.sml","",typeErrTT);
   ("exF17.sml","",typeErrTT)   
   ]
                    
// Compile and run tests
let testRes = List.concat (doAllTestsAllTestTypes tests)

verbose <- true

println "Result of test."
List.map (fun (testType,(f,ok,_,_,_)) -> println (f + "(" + (ppTestType testType) + "): " + ok)) testRes

