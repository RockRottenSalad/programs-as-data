(* Type error - let val x = er in eb end, rule g10 *)
(* x used with different type in body.             *)

begin
  let
    val x = 42
  in
    x && true
  end
end