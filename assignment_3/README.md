
# Exercise 4.2 - Write more example progrms in the functional language, and test them in the same way as in Exercise 4.1

- Computer the sum of the numbers from 1000 down to 1. Do this by defining a function `sum n` that computes the sum `n + (n - 1) + ... + 2 + 1.`

```
let sum n = if n < 1 then 0 else n + sum (n-1) in sum 1000 end
```

- Compute the number 3^8, that is, 3 raised to the power 8. Again, use a recursive function.

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in powerOf3 8 end
```

- Compute `3^0 + 3^1 + ... + 3^10 + 3^11`, using a recursive function (or two, if you prefer)

```
let powerOf3 n = if n = 0 then 1 else 3 * powerOf3 (n-1) in 
let sumPowersOf3 n = if n = 0 then 1 else sumPowersOf3 (n-1) + powerOf3 n in
sumPowersOf3 11
end end
```

- Compute `1^8 + 2^8 + ... + 10^8`, again using a recursive function (or two).


```
let eigthPower n = n*n*n*n*n*n*n*n in 
let sumRaisedToEigth n = if n = 1 then 1 else sumRaisedToEigth (n-1) + eigthPower n in
sumRaisedToEigth 10
end end
```


