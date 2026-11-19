(* Type error - try e1 with exn -> e2, rule g7                *)
(* exn is not an exception variable - leading to parse error. *)

begin
  try 42 with 42 -> 42
end