
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

> In this exercise we consider the concept of most specific method signature, see Sect. 1.4.3.

> We solve the exercise assuming this class hierarchy:
> Object <- Main
> <- U <- V
> <- S <- T <- H


> Consider the method environment Ψ below:
> • ( f , Object × U × S) : Object × U × S → T
> • ( f , U × Object × S) : U × Object × S → S
> • ( f , V × Object × T) : V × Object × T → S
> • ( f , U × U × S) : U × U × S → S

> For each actual method signature below, explain whether a most specific method signature exists in Ψ .

1. ( f , U × Object × S)

There exists such a function.

The following function is a 1:1 match:

 • ( f , U × Object × S) : U × Object × S → S


2. ( f , U × U × S)

There exists such a function.

The following function is a 1:1 match:

 • ( f , U × U × S) : U × U × S → S

3. ( f , U × Object × T)

There does not exist such a function.

Recall that `T` is a sub-type of `S`.

So this function matches.

• ( f , U × Object × S) : U × Object × S → S

4. ( f , V × V × H)

There does not exist such a function.


There are two functions which have signatures that match. However, looking at
the distance of each argument to the types given in the function call, we can
see that neither of the signatures is closer to the given signature than the
other.

Hence, there is no obvious function to call here.

1, 1, 2
• ( f , U × U × S) : U × U × S → S

0, 2, 1
• ( f , V × Object × T) : V × Object × T → S

5. ( f , null × null × null)

There does not exist such a function.
This could refer to literally any of the 4 functions.

6. ( f , Object × Object × Object)

There does not exist such a function.

None of the signatures match since `Object` is too generic with all of the signatures.

7. ( f , Object × S × U)

There does not exist such a function.

This simply doesn't match any of the signatures.

