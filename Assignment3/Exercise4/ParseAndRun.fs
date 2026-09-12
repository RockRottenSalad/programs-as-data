(* File Fun/ParseAndRun.fs *)

module ParseAndRun

let fromString = Parse.fromString;;

let p1 = "let sum n = if n = 0 then 0 else n + sum (n - 1) in sum 1000 end" (* summation program *)

let p2 =
    @"let exp x =
        let aux n =
            if n = 0
            then 1
            else x * aux (n - 1)
        in aux 8
        end
      in exp 3
      end"
    
let p3 =
    @"let sum base =
        let aux1 max =
            let aux2 curr =
                let exp n =
                    if n = 0
                    then 1
                    else base * exp (n - 1)
                in
                    if max < curr
                    then 0
                    else exp curr + aux2 (curr + 1)
                end
            in aux2 0
            end
        in aux1 11
        end
      in sum 3
      end"
    
let p4 =
    @"let sum max =
        let aux1 curr =
            let exp n =
                if n = 0
                then 1
                else curr * exp (n - 1)
            in
                if max < curr
                then 0
                else exp 8 + aux1 (curr + 1)
            end
        in aux1 1
        end
    in sum 10
    end
    "

// examples from 4.4
let idk1 =
    @"
        let max2 a b = if a<b then b else a
        in let max3 a b c = max2 a (max2 b c)
        in max3 25 6 62 end
        end
    "
    
let idk2 = "let pow x n = if n=0 then 1 else x * pow x (n-1) in pow 3 8 end"

let eval = Fun.eval;;

let run e = eval e [];;
