(* Type error on circularity            *)
(* Happens in TypeInference.occurCheck. *)

begin
  let
    fun f g = g g
  in
    f
  end
end