(* Testing Tail Recursion *)

fun id x = x

fun t1 x = id x         (* Tail call *)
fun t2 x = id x + id 5  (* No tail calls *)
fun t3 x = id x ; id 5  (* Second is tail call *)
val t4 = fn x -> id x   (* Tail call *)
fun t5 x = t4 (id x)    (* Normal call to id *)
exception exn
fun t6 x = try t4 x with exn -> id x  (* Tail call to id *)
begin
  print t1 4;
  print t2 4;
  print t3 4;
  print t4 4;
  print t5 4;
  print t6 4
end

