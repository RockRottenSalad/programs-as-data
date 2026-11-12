(* Show example of several functions having same free variable. *)
(* Motivating shared closure used in exercise.                  *)

val x = 42
fun f y = y + x 
and g y = y - x

begin
  print (f 43);
  print (g 41)
end
