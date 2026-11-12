(* Type error - mutable recursive functions, rule g12    *)
(* A function used with incompatible type instantiation. *)

begin
  let
    fun f x = x + 42
    and g x = f x && true
  in
    f 1
  end
end