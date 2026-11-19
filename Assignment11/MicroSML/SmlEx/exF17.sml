(* Type check - result type of print is same as argument *)
(* Result type bool from print does not unify with int.  *)

begin
  print true + 42
end  