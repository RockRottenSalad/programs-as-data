// See F# language specification for details on polymorphic recursion.
// https://fsharp.github.io/fslang-spec/inference-procedures/
// Section 14.6
// Run from SmlEx folder:
//   dotnet fsi
//   #load "polyRecursive.fsx";;

let rec f<'a> (x:'a) : 'a =
  let a = g 1
  let b = g false
  x
and g<'b> (x:'b) : 'b =
  let a = f 1
  let b = f true
  x

