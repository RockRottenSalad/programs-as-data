# 6.5
### (1)
> Use the type inference on the micro-ML programs shown below, and report what type the program has. Some of the type inferences will fail because the programs are not typable in micro-ML; in those cases, explain why the program is not typable

The following fails:
```
let f g = g g in f end
``` 
[insert explanation]

The following also fails:
```
let f x =
let g y = if true then y else x
in g false end
in f 42 end
```
this fails due to `x` having type `int` while `y` has type `bool`. because `y` is a ``bool`` the function `g` expects the function type to be `g: bool -> bool` however `x` is given type `int`. 
### (2)
>Write micro-ML programs for which the micro-ML type inference report the following types:

1. bool -> bool

**Solution:**
```F#
let f x = if x then x else false in f end
```
2. int -> int

**Solution:**
```F#
let f x = x + x in f end
```

3. int -> int -> int

**Solution:**
```F#
let f x = let g y = x + y in g end in f end
//result: "(int -> (int -> int))"
```
4. ’a -> ’b -> ’a

**Solution:**
```F#
let f x = let g y = x + y in g end in f end
//result: "('h -> ('g -> 'h))"
```

5. ’a -> ’b -> ’b
**Solution:**
```F#
let f x = let g y = x in g end in f end
//result: "('h -> ('g -> 'h))"
```
6. (’a -> ’b) -> (’b -> ’c) -> (’a -> ’c)

**Solution:**
```F#
let f x = let g y = let h z = y (x z) in h end in g end in f end
//result: "(('l -> 'k) -> (('k -> 'm) -> ('l -> 'm)))"
```
7. ’a -> ’b

**Solution:**
```F#
let f x = f x in f end
//result: "('e -> 'f)"
```
8. ’a

**Solution:**
```F#
let f x = f x in f f end
//result: "'f"
```