(* Used in exercise

   The purpose of alpha conversion is to make all variables unique
   which simplifies the calculation of free variables.

   The below expression is converted into
   let
     val x0 = 2
     val x1 = 5
   in
     x1 + x1
   end
*)

begin
  let
    val x = 2
    val x = 5
  in
    x + x
  end
end

