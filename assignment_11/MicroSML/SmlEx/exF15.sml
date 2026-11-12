(* Type error - mutable recursive functions, rule g12 *)
(* Polymorphic recursing not supported.               *)
(* Compare with polyRecursive.fsx                     *)

begin
  let
    fun f x =
      let 
        val a = g 1
        val b = g false
      in
        x
      end
    and g x =
      let
        val a = f 1
        val b = f true
      in
        x
      end
  in
    f 1
  end
end