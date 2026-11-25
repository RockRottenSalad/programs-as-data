# Exercise 13.3
> Implement alpha conversion
> 
> a) Implement the function alphaConv. Using the template is optional.

For the implementation we have used the provided template. The result of implementation is the following:
```fsharp
let alphaConv p : program<'a,'b> =
  let mutable varNo = 0
  let getFreshVar (x: string) =
    varNo <- varNo + 1
    x + string(varNo)
    
  let rec lookup x =
    function
    | [] -> failwith "could not find variable " + x
    | (y, v) :: xs -> if y = x then v else lookup x xs
  
  let rec aExpr env e =
    match e with
    | CstI _ | CstB _ | CstN _ -> e
    | Var(s, opt) -> Var (lookup s env, opt)
    | AndAlso(e1, e2, opt) -> AndAlso (aExpr env e1, aExpr env e2, opt)
    | OrElse(e1, e2, opt) -> OrElse (aExpr env e1, aExpr env e2, opt)
    | Seq(e1, e2, opt) -> Seq (aExpr env e1, aExpr env e2, opt)
    | Prim2(op, e1, e2, opt) -> Prim2 (op, aExpr env e1, aExpr env e2, opt)
    | Prim1(op, e1, opt) -> Prim1 (op, aExpr env e1, opt)
    | If(guard, e1, e2) -> If (aExpr env guard, aExpr env e1, aExpr env e2)
    | Fun(x, fbody, opt) ->
      let x' = getFreshVar x
      let env' = (x, x') :: env
      Fun (x', aExpr env' fbody, opt)
    | Call(e1, e2, bOpt, opt) -> Call (aExpr env e1, aExpr env e2, bOpt, opt)
    | Let(valdecs, letBody) ->        
      let (valdecs', env') = valdecshelper valdecs env
      Let(valdecs', aExpr env' letBody)
    | Raise(e1, opt) -> Raise (aExpr env e1, opt)
    | TryWith(e1, exn, e2) -> TryWith (aExpr env e1, exn, aExpr env e2)
  and aValdec' env = function
    | Valdec(x,eRhs, opt) ->
      let x' = getFreshVar x
      let env' = (x, x') :: env
      (Valdec (x', aExpr env eRhs, opt), env')
    | Fundecs fundecs ->
      //  Fundecs fs -> Fundecs(List.map (fun (f,x,e,bOpt) -> (f,x,tc' true e,bOpt)) fs)
      // each mutual recursive function should get a new label
      // afterwards we should make sure to process each of their bodies in the env where every one has been declared
      let newFunctionNames = List.map (fun (f, _, _, _) -> (f, getFreshVar f)) fundecs
      let env' = newFunctionNames @ env
      
      let map = List.zip fundecs newFunctionNames
      let newDecs = map |> List.map (
        fun ((_,x,e,bOpt), (_, f')) ->
          let x' = getFreshVar x
          let env'' = (x, x') :: env'
          (f', x', aExpr env'' e, bOpt)
        )
      
      Fundecs newDecs, env'
    | Exndec exn -> (Exndec exn, env)
  and aProg' = function
    | Prog (valdecs,body) ->
      let (valdecs', env') = valdecshelper valdecs []
      Prog (valdecs', aExpr env' body)
  and valdecshelper valdecs env =
    match valdecs with
    | [] -> ([], env)
    | valdec :: xs ->
      let (idk, env') = aValdec' env valdec
      let (rest, env'') = valdecshelper xs env'
      (idk :: rest, env'')
  aProg' p
```
There are a lot of constructors that are trivial, so we won't be discussing them in detail. The ones of interest are discussed below:
```fsharp
| Var(s, opt) -> Var (lookup s env, opt)
```
If we have a `Var "x"` we naturally wan't to tranform it into the alpha converted version, so we perform a lookup to find the most recently bound variable.

```fsharp
    | Fun(x, fbody, opt) ->
      let x' = getFreshVar x
      let env' = (x, x') :: env
      Fun (x', aExpr env' fbody, opt)
```
If we have a function `fun x -> body`, then first we want to get a fresh name for the argument. Afterwards, we then want to alpha convert the body of the function in an environment, where `x` has been bound to the fresh variable.

Before discussing `aValdecs` and `let` we have to discuss the implementation of `valdecshelper`:
```fsharp
  and valdecshelper valdecs env =
    match valdecs with
    | [] -> ([], env)
    | valdec :: xs ->
      let (idk, env') = aValdec' env valdec
      let (rest, env'') = valdecshelper xs env'
      (idk :: rest, env'')
```
It is quite simple. It takes a list of `valdecs`. Then for each valdec it does:
1. Alpha convert the `valdec`
2. Get the resulting `valdec` and the environment
3. Evaluate the next `valdec` but in the new environment
The reason why we want this is because imagine you have:
```fsharp
begin
    let
        val x = 2
        val y = 2 + x
    in
        x
end
```
We want to be able to use the alpha converted `x` when we alpha convert `y`.

Now, `aValdec` is also relatively simple:
```fsharp
  and aValdec' env = function
    | Valdec(x,eRhs, opt) ->
      let x' = getFreshVar x
      let env' = (x, x') :: env
      (Valdec (x', aExpr env eRhs, opt), env')
    | Fundecs fundecs ->
      let newFunctionNames = List.map (fun (f, _, _, _) -> (f, getFreshVar f)) fundecs
      let env' = newFunctionNames @ env
      
      let map = List.zip fundecs newFunctionNames
      let newDecs = map |> List.map (
        fun ((_,x,e,bOpt), (_, f')) ->
          let x' = getFreshVar x
          let env'' = (x, x') :: env'
          (f', x', aExpr env'' e, bOpt)
        )
      
      Fundecs newDecs, env'
    | Exndec exn -> (Exndec exn, env)
```

The first case where we have a simple `Valdec`:
```fsharp
    | Valdec(x,eRhs, opt) ->
      let x' = getFreshVar x
      let env' = (x, x') :: env
      (Valdec (x', aExpr env eRhs, opt), env')
```
Simple gets a fresh variable for `x`, instantiates a new environment where `x` is mapped to the fresh variable, and then we
alpha convert the right hand side of the evaluation in the previous environment (note!)
- the reason we do it in the previous environment is because `let x = x` is only valid if `x` existed before.
and then we return a tuple of the new valdec and the new environment.

`Fundecs funcdecs` is a bit more advanced
```fsharp
    | Fundecs fundecs ->
      let newFunctionNames = List.map (fun (f, _, _, _) -> (f, getFreshVar f)) fundecs
      let env' = newFunctionNames @ env
      
      let map = List.zip fundecs newFunctionNames
      let newDecs = map |> List.map (
        fun ((_,x,e,bOpt), (_, f')) ->
          let x' = getFreshVar x
          let env'' = (x, x') :: env'
          (f', x', aExpr env'' e, bOpt)
        )
      
      Fundecs newDecs, env'
```
`Fundecs` represents mutually recursive functions, and the algorithm converts it in two steps.
1. The first step is to ensure that every function gets a fresh name, and that the environment `env'` is extended to include all of the fresh names, so every function body can call any function in the group of mutually recursive function.
2. For each function, we create a fresh name for its argument, extend the environment `env'` with this fresh name locally, and then alpha convert the function body in this environment.

Since the function arguments are only in scope inside their own functions, we have made sur to return the environment where only the fresh function names are mapped.

Now with these discussed, it is possible to make the `let` rule:
```fsharp
    | Let(valdecs, letBody) ->        
      let (valdecs', env') = valdecshelper valdecs env
      Let(valdecs', aExpr env' letBody)
```
The let rule alpha converts all its variable declarations `valdecs` using the helper discussed above, and then it alpha converts its body in the environment where the `valdecs` have been alpha converted.

> (b) Compile and run example ex11.sml. By inspecting the abstract syntax, verify that alpha conversion gives the expected result. Verify that you can run the program with expected result.

Compiling the example without alpha conversion yields:
```fsharp
PS C:\Users\August\Desktop\programs-as-data\Assignment11\MicroSML> dotnet run -verbose SmlEx/ex11.sml    
    let
      val y = 1
      fun f x = 
        let
          val z = (y + 1)
          val y = 1
        in
          ((z + y) + x)
        end
    in
      f y
    end
end
```

With alpha conversion we get:
```fsharp
> PS C:\Users\August\Desktop\programs-as-data\Assignment11\MicroSML> dotnet run -alpha -verbose SmlEx/ex11.sml   
begin
    let
      val y1 = 1
      fun f2 x3 = 
        let
          val z4 = (y1 + 1)
          val y5 = 1
        in
          ((z4 + y5) + x3)
        end
    in
      f2 y1
    end
end
```
And by inspecting the abstract syntax we can see that the alpha conversion works. It correctly maps each variable to a fresh name, and uses the correct fresh names when reffering to the variables. A prime example is in:

We can verify that it gives the expected result by interpreting it:
```fsharp
PS C:\Users\August\Desktop\programs-as-data\Assignment11\MicroSML> dotnet run -eval -alpha  SmlEx/ex11.sml 
Micro-SML compiler v 2.0 of 2026-05-30
Compiling SmlEx/ex11.sml to SmlEx/ex11.out.


Evaluating Program

Result value: 4 
Used: Elapsed 5ms, CPU 0ms
```
Which is what you would expect from the program.
