(* Used in exercise

   Will not compile without alpha conversion.
*)

begin
  let
    exception exn
    fun g x =
      let
        val x = try print 42 with exn -> print 43
        exception exn
      in
        x
      end
  in
    g 42
  end
end
