// Command-line compiler for micro-SML with compiler options.

// Building the command-line compiler for micro-SML with parameters
//   * -verbose : print verbose information on stdout
//   * -eval : evaluate the program
//   * -alpha : do alpha conversion
//   * -opt : compile with optimizations enabled (includes tail calls)
//   * -debug: enambles the debug function in Contcomp.fs for debugging.

open Absyn

let _ = printf "Micro-SML compiler v 2.0 of 2026-05-30\n"

let ppType = function
    None -> ""
  | Some t -> ":" + (TypeInference.showType t)

let ppTypeScheme = function
    None -> ""
  | Some ts -> ":" + (TypeInference.showTypeScheme ts)

let ppFreevars = function
    None -> ""
  | Some env -> ": [" + (String.concat "," env) + "]\n"

let ppNoType _ = ""
let ppNoTypeScheme _ = ""

let _ =
  try
    let cmdL = CmdLine.readCmdParams()
    CmdLine.verbose (Util.ppSysInfo())
    printfn "Compiling %s to %s.\n" cmdL.source cmdL.target

    // Phase lexing and parsing
    let program = Parse.fromFile cmdL
    CmdLine.verbose (sprintf "Program after parsing: \n%s" (Absyn.ppProg ppNoType ppNoTypeScheme program))
    CmdLine.debug (sprintf "Program AST after parsing: \n%A" program)

    // Phase alpha transformation
    let pAlpha =
      if CmdLine.chkArg CmdLine.alphaArg
        then let pAlpha = Absyn.alphaConv program
             CmdLine.verbose (sprintf "Program after alpha conversion (exercise): \n%s"
                                      (Absyn.ppProg ppNoType ppNoTypeScheme pAlpha))
             CmdLine.debug (sprintf "Program AST after alpha conversion (exercise): \n%A" pAlpha)
             pAlpha
        else program

    // Phase tailcall annotation
    let pTailCall =
      if CmdLine.chkArg CmdLine.optArg
        then let pTailCall = Absyn.tailcalls pAlpha
             CmdLine.verbose (sprintf "Program with tailcalls: \n%s" (Absyn.ppProg ppNoType ppNoTypeScheme pTailCall))
             CmdLine.debug (sprintf "Program with tailcalls: \n%A" pTailCall)
             pTailCall
        else pAlpha

    // Phase type inference
    let (typ',_,pTyp) = TypeInference.inferProg pTailCall
    CmdLine.verbose (sprintf "Program with types:\n%s" (Absyn.ppProg ppType ppTypeScheme pTyp))
    CmdLine.verbose (sprintf "Result type: %s" (TypeInference.showType typ'))
    CmdLine.debug (sprintf "Program with types:\n%A" pTyp)
    CmdLine.debug (sprintf "Result type: %A" typ')

    // Phase interpretation 
    let _ = if CmdLine.chkArg CmdLine.evalArg
              then printf "\nEvaluating Program\n";
                   let (r,cpu,elapsed) = Util.cpuTime (HigherFun.evalProg []) pTyp
                   printf "\nResult value: %s \n" (HigherFun.ppAnswer r);
                   printf "Used: Elapsed %dms, CPU %dms" elapsed (int64 cpu)
            else ()

    // Phase compilation
    let instrs = Comp.cProgram pTyp
    CmdLine.debug (sprintf "Bytecode: \n%s" (Machine.ppInstrs instrs))
    CmdLine.verbose (sprintf "Compiled program has %d bytecode instructions.\n"
                             (List.length (List.filter (fun i -> Machine.sizeInstr i > 0) instrs)))

    // Compile into machine code
    let ints = Machine.code2ints CmdLine.debug instrs

    // Emit machine code
    Machine.intsToFile ints cmdL.target

    ()

  with Failure eMsg -> printf "%s\n" eMsg
