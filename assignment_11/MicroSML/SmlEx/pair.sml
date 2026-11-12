val p = (1,43)
fun f p = if fst(p) < 0 then g p else f (fst(p)-1, snd(p))
and g p = (fst(p),snd(p)-1)

begin
  print (f p)
end
