(* The global exception variable __enxNum__ is used implicitly when
   declaring an exception.  Calculating free variables for an
   exception declaration will include the __exnNum__ variable.

   Test tat it works for an anonymous function. *)

begin
  let
    val genE = fn x -> let exception E in E end
  in
    raise (genE 1)
  end
end
