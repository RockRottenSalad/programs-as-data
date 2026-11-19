(* The global exception variable __enxNum__ is used implicitly when
   declaring an exception.  Calculating free variables for an
   exception declaration will include the __exnNum__ variable.

   Test tat it works for a named function. *)
   
fun genExn x = let exception E in E end

begin
  let
    val E = genExn 1
  in
    raise E
  end
end
