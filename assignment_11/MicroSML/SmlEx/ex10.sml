(* Example showing the calculation of free variables 
   Try with -debug option *)

val n = 42

begin
  let
    val x = 1                 (* fvs=[]    bvs=[x]         *)
    val y = x + 1             (* fvs=[x]   bvs=[x,y]       *)
    val z = x + y             (* fvs=[x,y] bvs=[x,y,z]     *)
    fun f x = x + y + n       (* fvs=[n,y] bvs=[x,y,z,f]   *)
    val g = fn x -> z + (f x) (* fvs=[f,z] bvs=[x,y,z,f,g] *)
  in
    x+y
  end         
end 
