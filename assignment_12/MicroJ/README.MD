# 1.2
>Consider the subtype rules in Fig. 1.7. [...]
Determine which of these subtype relations are valid:
>H <: T

true, because `T <- H`
>U <: S

false, because 
>Main <: Object

true, due to all classes inheriting from class Object

>Object <: Main 

false, other way around
>null <: V

true because null is a subtype of all types
>V <: null

false, other way around
>S <: H

false, other way around
>void <:λ void

true because that one rule which says so
>int <:λ void

false
>H <:λ void

false
>T <:λ H

false, other way around
# 1.4
>For each actual method signature below, explain whether a most specific method signature exists in Ψ .

>1. ( f , U × Object × S)

matches with:
( f , U × Object × S) : U × Object × S → S
>2. ( f , U × U × S)

matches with:
( f , U × U × S) : U × U × S → S
>3. ( f , U × Object × T)

le bad
>4. ( f , V × V × H)

le bad
>5. ( f , null × null × null)

le bad
>6. ( f , Object × Object × Object)

le bad
>7. ( f , Object × S × U)

le bad