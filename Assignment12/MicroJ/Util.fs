// Utilities for compiler micro-Java.

module Util

// System agnostic newline.
let nl = System.Environment.NewLine

// Panic failure.
let fatal msg = failwith msg

// Indent string for pretty printing.
let indent i s = System.String(' ',i) + s

// PadLeft on string for pretty printing.
let padLeft l s =
  let spaces = l - String.length s
  if l > 0 then indent spaces s else s

// Create folder if not existing, e.g. for test folders.
let ensureDir path =
  ignore (System.IO.Directory.CreateDirectory(path))

// Write file in dir - create dir if not existing.  
let writeFile dir filename (str:string) =
  ensureDir dir
  let path = System.IO.Path.Combine(dir, filename)
  System.IO.File.WriteAllText(path, str)

// Folding including index with first element at index 0.
// Not in standard F# List module.
let foldi f init xs =
  let rec loop i acc = function
    | [] -> acc
    | x::xs -> loop (i+1) (f i acc x) xs
  loop 0 init xs

// Add element to list if does not already exists based on predicate
// Not standard function in list library - and not efficient at all.
let addUniquely p x xs =
  if List.exists (p x) xs then xs
  else x :: xs

// Checks that elements in a list is ordered according to comparer function
let rec isOrdered p = function
    []  -> true
  | [_] -> true
  | x::y::xs -> p x y && isOrdered p (y::xs)

// Number generator
let numGen = ref 0
let newNum() =
  numGen.Value <- numGen.Value + 1
  numGen.Value - 1
                         
// Detect and print operating system, architecture and .net version.  
let ppSysInfo() =
  "System Information" + nl +
  sprintf "  Framework: %s" System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + nl +
  sprintf "  Environment.Version: %O" System.Environment.Version + nl +
  sprintf "  OS: %s" System.Runtime.InteropServices.RuntimeInformation.OSDescription + nl +
  sprintf "  OS Acrhitecture: %A" System.Runtime.InteropServices.RuntimeInformation.OSArchitecture
