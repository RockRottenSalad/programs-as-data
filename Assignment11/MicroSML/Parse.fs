// File microSML/Parse.fs

// Lexing and parsing of micro-SML programs using fslex and fsyacc

module Parse

open System
open System.IO
open System.Text
open FSharp.Text.Lexing
open Util
open CmdLine
open Absyn

// Plain parsing from a string, with poor error reporting
let fromString (str : string) : program<'a,'b> =
  let lexbuf = LexBuffer<char>.FromString(str)
  try 
    FunPar.Main FunLex.Token lexbuf
  with 
  | exn -> let pos = lexbuf.EndPos 
           fatal "%s near line %d, column %d\n" (exn.Message) (pos.Line+1) pos.Column
                             
// Parsing from a file
let fromFile cmdL =
  verbose ("Parsing file " + cmdL.source)
  try
    use reader = new StreamReader(cmdL.source)
    let lexbuf = LexBuffer<char>.FromTextReader reader
    try 
      let abs = FunPar.Main FunLex.Token lexbuf
      abs
    with 
    | exn -> let pos = lexbuf.EndPos 
             fatal (sprintf "%s in file %s near line %d, column %d\n"
                            (exn.Message) cmdL.source (pos.Line+1) pos.Column)
  with
    | exn -> fatal (sprintf "%s." (exn.Message))

