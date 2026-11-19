(* Type error - let exception exn in eb end, rule g11 *)
(* exn used in non exception context in body.         *)

begin
  let
    exception exn
  in
    exn + 42
  end
end