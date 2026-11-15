(* Type error - try e1 with exn -> e2, rule g7 *)
(* e1 and e2 not of same type. *)

exception exn
begin
  try 42 with exn -> true
end