(* Used in exercise

   Will not compile without alpha conversion.
*)

begin
  let
    fun f x = x + 2
    fun g x =
      let 
        val x = f 32
        fun f x = x + 3
      in
        x
      end
  in
    g 42
  end
end
