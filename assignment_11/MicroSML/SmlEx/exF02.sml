(* Type error - x and y are unified and must have same type *)
(* y is bool, x is int - can't be unified.                  *)

begin
  let
    fun f x =
      let
        fun g y =
	  if true then y
	          else x
      in
        g false
      end
  in
    f 42
  end
end