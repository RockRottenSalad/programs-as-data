// File microJ/CmdLine.fs 

// Representation of command line arguments for compiler, micro-Java

module CmdLine

open Util

type cmdLine =
  { compiler: string
    source: string;
    target: string;
    args: Set<string>}

let empty() =
  { compiler = "";
    source = "";
    target = "";
    args = Set.empty}

let verboseArg = "-verbose"
let debugArg = "-debug"
let genJavacArg = "-genJavac"
let allArgs = set [verboseArg; debugArg; genJavacArg]

let globalCmdLine = ref (empty())

let chkArg arg = Set.exists ((=) arg) (!globalCmdLine).args

let argMsg arg title msg =
  if Set.exists ((=) arg) (!globalCmdLine).args
    then printf "%s" (nl + title + " " + msg + nl)
    else ()

let debug msg = argMsg debugArg "DEBUG" msg
let verbose msg = argMsg verboseArg "VERBOSE:" msg
let genJavacFn fnGen = if chkArg genJavacArg then argMsg genJavacArg "GENJAVAC: " (fnGen()) else ()
let panic msg = failwith (sprintf "%s" (nl + "PANIC:" + msg + nl))

let ppUsage allArgs =
  let ppArgs = Set.fold (fun acc arg -> acc + " [" + arg + "]") "" allArgs
  "Usage: dotnet run" + ppArgs + " <source file>" + nl

let readCmdParams () =
  let cmdArgs = System.Environment.GetCommandLineArgs()          
  let compiler = cmdArgs[0]   // There is always at least the program name run.
  let cmdArgs = cmdArgs[1..]  // Do not include program name run.
  let args = Set.intersect (Set.ofArray cmdArgs) allArgs
  let cmdL = 
    if Array.length cmdArgs > 0 then
      // Assume source is always last argument.
      let source = Array.last cmdArgs
      let stem =
        if source.EndsWith(".java") then source.Substring(0,source.Length-5) 
        else source
      let target = stem + ".out"
      {compiler=compiler; source=source; target=target; args=args}
     else
      raise (Failure (ppUsage allArgs))
  globalCmdLine := cmdL
  cmdL
