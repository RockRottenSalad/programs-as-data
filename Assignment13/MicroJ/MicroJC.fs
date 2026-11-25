// Command-line compiler for Micro-Java with compiler options

// Building the command-line compiler for Micro-Java with parameters
//     * -verbose : print verbose information on stdout
//     * -debug: enambles the debug function in Contcomp.fs for debugging.
//     * -genJavac: generate java files to be compiled with javac.

open Absyn

let _ = printf "Micro-Java compiler v 1.00 of 2026-05-14\n"

let _ =
  try
    let cmdL = CmdLine.readCmdParams()
    CmdLine.verbose (Util.ppSysInfo())
    printfn "Compiling %s to %s.\n" cmdL.source cmdL.target

    // Phase lexing and parsing
    let program = Parse.fromFile cmdL
    CmdLine.verbose (sprintf "Program after parsing: %s" (Absyn.ppProg false false program))
    CmdLine.debug (sprintf "Program AST after parsing: %A" program)

    // Generate javac version if -genJavac provided.
    CmdLine.genJavacFn (fun () ->  genJavac program)

    // Phase return and reachability analysis
    let p' = rraProg program
    CmdLine.verbose (sprintf "Program after Return and Reachability Analysis: %s" (Absyn.ppProg false false p'))
    CmdLine.debug (sprintf "Program AST after Return and Reachability Analysis: %A" p')

    // Phase build class hierarchy
    let ch = buildClassHierarchy p'
    CmdLine.verbose (sprintf "Class hierarchy: \n%s\n" (ppClassHierarchy ch))    
    CmdLine.verbose (sprintf "Program as class hierarchy: \n%s\n" (ppCH false ch))

    // Phase type check
    let chWithTypes = Type.typProg ch
    CmdLine.verbose (sprintf "Program with types as class hierarchy: \n%s\n" (ppCH true chWithTypes))
    CmdLine.debug (sprintf "Program AST with types as class hierarchy: \n%A\n" chWithTypes)

    // A-normalize - after type check such that return type on method calls are known.
    let chAnorm = aNormGC_CH chWithTypes
    CmdLine.verbose (sprintf "Program after A-normalization for GC: %s" (ppCH true chAnorm))
    CmdLine.debug (sprintf "Program AST after A-normalization for GC: %A" chAnorm)

    // Compile into byte code
    let instrs = Comp.cProg cmdL chAnorm 
    CmdLine.debug (sprintf "Bytecode: \n%s" (Machine.ppInstrs instrs))    
    CmdLine.verbose (sprintf "Compiled program has %d bytecode instructions.\n"
                             (List.length (List.filter (fun i -> Machine.sizeInstr i > 0) instrs)))

    // Compile into machine code
    let ints = Machine.code2ints CmdLine.debug instrs

    // Emit machine code
    Machine.intsToFile ints cmdL.target

    ()
    
  with Failure msg -> printf "%s\n" msg
