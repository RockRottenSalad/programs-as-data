(* Demonstrating need for temporary global variables, Comp.GloTmpvar *)
(* A global temporary variable is accessed as an absolute address on *)
(* stack - but only for the temporary computation.                   *)
(* Relevant for let expressions part of global expressions.          *)

val f =
  let
    val y = 42
  in
    fn x -> x + y
  end
  
begin
  print f 1
end