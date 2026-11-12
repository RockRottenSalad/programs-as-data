(* Example showing the calculation of free variables 
   Try with -debug option *)

(* This example fails because the function freevarsValdec in file
   Absyn.fs does not work for programs redeclaring variables with same
   name. You can only add a variable x to bvs in a let expression, if
   a variable of same name has not been reported free in an earlier
   value declaration within the let expression.

   The variable y in the value declaration val z = y + 1, is free and
   should be part of the closure of f. A variable with same name is
   declared in the same let expression val y = 1, which makes it part
   of bvs2, and therefore not part of the free variables for f and not
   in the closure.

   This is solved in an exercise implementing alpha conversion.
 *)

begin
  let
    val y = 1          (* fvs1=[]   bvs1=[y]   *)
    fun f x =          (* fvs2=[y]  bvs1=[y,f] *)
      let
        val z = y + 1  (* fvs2=[y]  bvs2=[z]   *)
        val y = 1      (* fvs2=[]   bvs2=[z,y] *) 
      in
        z+y+x 
      end              
  in
    f y
  end         
end 
