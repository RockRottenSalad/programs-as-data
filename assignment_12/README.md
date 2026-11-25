
# Exercise 1.1

> Build the micro-Java compiler as explained in file README.
> Compile example programs ex01.java and ex02.java.
> Use the -verbose command line parameter and get familiar with

> • the program after parsing
> • the program after return and reachability analysis
> • the class hierarchy
> • the program as a class hierarchy
> • the class table
> • the program as a class hierarchy with type annotations

We've done this. Not much to say here.

# Exercise 1.2

> Consider the subtype rules in Fig. 1.7.

> Also, assume this class hierarchy exists:
> Object <- Main
> <- U <- V
> <- S <- T <- H

> Determine which of these subtype relations are valid:


1. H <: T

True, since we can see that `T <- H`, i.e. `H` inherits from `T`.


2. U <: S

False, since `U` does not inherit `S`.

3. Main <: Object

True, since all classes are a subtype of `Object`.

4. Object <: Main

This is false.

`Object` is at the very top of the class hierarchy, it can never be a subtype of anything else

`Main <: Object` is true, this is not.

5. null <: V

True by the following rule:

```
    class A extends ... {...}
--------------------------------
         null <: A
```

6. V <: null

This is false.

There does not exist a rule that makes this true.

7. S <: H

This is false.

We can see that `H` is a sub-type of `T`, but `T` is not a sub-type of `H`.

`Object <- S <- T <- H`

8. void <:λ void

True.

Due to this rule:

```
--------------------
   void <:λ void
```

9. int <:λ void

False.

There does not exist a rule that makes this true. There is no other type that is a sub-type of `void`.

10. H <:λ void

False for the same reason as 9.

11. T <:λ H

False.

`T` is not a sub-type of `H` However, the opposite is true, `H` is a sub-type of `T`.


`Object <- S <- T <- H`

# Exercise 1.4
> In this exercise we consider the concept of most specific method sig-
> nature, see Sect. 1.4.3. We solve the exercise assuming this class hierarchy:
> ```
> Object <- Main
> <- U <- V
> <- S <- T <- H
> ```
> Consider the method environment below (Ψ):
> - `( f , Object × U × S) : Object × U × S → T`
> - `( f , U × Object × S) : U × Object × S → S`
> - `( f , V × Object × T) : V × Object × T → S`
> - `( f , U × U × S) : U × U × S → S`
> 
> For each actual method signature below, explain whether a most specific method
    signature exists in Ψ .

1. `( f , U × Object × S)`
    
    There exists a most specific signature in Ψ, namely `( f , U × Object × S) : U × Object × S → S`.
2. `( f , U × U × S)`

    There exists a most specific signature, namely `( f , U × U × S) : U × U × S → S`.

3. `( f , U × Object × T)`

    There exists a most specific function, namely `( f , U × Object × S) : U × Object × S → S`. This is valid, since `T <: S`, so we can use `T` in place of `S`.
    
    One could think that there are two other possible functions, but these are invalid, since they give an argument that is less specific that the expected argument.

4. `( f , V × V × H)`
    
    There is no most specific functions.

    There are to functions that possibly could match:
    - `( f , U × U × S)` 
    - `( f , V × Object × T)`
   
    The issue is, that for both these it does not hold that all the arguments are more specific that the other arguments, i.e. while it is the case that `U` is less specific that `V`, `U` is more specific than `Object`.

5. `( f , null × null × null)`

    Since `null` is a subtype is of all objects, it means that all the signatures potentially match.
    And there is no most specific one.

6. `( f , Object × Object × Object)`
    
    This is not an example of where there isnt a specific function, but an error where no signature actually matches the arguments. We can't give a less specific object, when we expect a more specific one. All the posible signatures expect some subtype of `Object` at some point, which means that they all reject this output.

7. `( f , Object × S × U)`

    There is no most specific function, since there is no signature that matches the call.
    The only possible candidate would be `(f, Object x U x S)`, but `S` is not a subtype of `U`, so it does not match.

