(* Type error - fn x -> e, rule g9            *)
(* x used with two different types in body e. *)

begin
  fn x -> (x + 42; x && true)
end