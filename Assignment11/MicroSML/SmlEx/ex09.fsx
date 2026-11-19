// Example ex09.sml converted to F\#
// Run with
//   dotnet fsi ex09.fsx

let rec f x = if x < 0 then g 4 else f (x-1)
and g x = x  

printfn "%d" (f 2)

