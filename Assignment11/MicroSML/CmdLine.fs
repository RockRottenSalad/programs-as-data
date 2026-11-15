// File microSML/CmdLine.fs 

// Representation of command line arguments for compiler, micro-SML

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

let optArg = "-opt"
let verboseArg = "-verbose"
let debugArg = "-debug"
let evalArg = "-eval"
let alphaArg = "-alpha"
let allArgs = set [optArg; verboseArg; debugArg; evalArg; alphaArg]

let globalCmdLine = ref (empty())

let chkArg arg = Set.exists ((=) arg) (!globalCmdLine).args

let argMsg arg title msg =
  if Set.exists ((=) arg) (!globalCmdLine).args
    then printf "%s" (nl + title + " " + msg + nl)
    else ()

let debug msg = argMsg debugArg "DEBUG" msg
let verbose msg = argMsg verboseArg "VERBOSE:" msg
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
        if source.EndsWith(".sml") then source.Substring(0,source.Length-4) 
        else source
      let target = stem + ".out"
      {compiler=compiler; source=source; target=target; args=args}
     else
      raise (Failure (ppUsage allArgs))
  globalCmdLine := cmdL
  cmdL
