# Exercise 1.1
> Build the micro-Java compiler as explained in file README.
Compile example programs `ex01.java and `ex02.java`.
Use the -verbose command line parameter and get familiar with

We can build the compiler by running the command:
```fsharp
dotnet build microjc.fsproj
```

We can compile examples by doing:
```fsharp
dotnet run JavaEx/ex01.java
```

The rest of the exercise has been done, we are very familiar with MicroJava now :)!!!!!!! (do not question our sincerity)

# Exercise 1.2
> Consider the subtype rules in Fig. 1.7. Also, assume this class hierarchy exists:
> ```fsharp
> Object <- Main
> <- U <- V
> <- S <- T <- H
> ```
>
> Determine which of these subtypes relations are valid:

1. `H <: T`
    This is valid using this rule:
    ```
    class H extends T {}
    --------------------
         H <: T
    ```
    This is apparent from the class hierachy given, i.e. `S <- T <- H`.

2. `U <: S`: This is invalid, since `U` does not extend `S` or any of its subtypes.
3. `Main <: Object`: Valid since `Object <- Main`.
4. `Object <: Main`: Invalid, since `Main <- Object` is not valid
5. `null <: V`: Valid, since null is a subtype of every class.
6. `V <: null`: Invalid
7. `S <: H`: Invalid
8. `void <:lambda void`: Valid
9. `int <:lambda void`: Invalid
10. `H <:lambda void`: Invalid
11. `T <:lambda H`: Invalid, since `H <: T` and not `T <: H`.

# Exercise 1.3

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

    