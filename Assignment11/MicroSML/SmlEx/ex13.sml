(* Testing generic print *)

fun output v = print v

begin
  print true;
  print 32;
  print nil;
  print 1::nil;
  print false::nil;
  print (fn x -> x+1);
  print output
end

