# exercise 7.1
> Extend the micro-C abstract syntax in Absyn.fs with the preincrement and predecrement operators [...] 
> Modify the micro-C interpreter in Interp.fs to handle PreInc and PreDec.
> You will need to modify the eval function, and use the getSto and setSto store operations

introduce the mentioned changes to `Absyn.fs`:
````fsharp
type expr =
  ...
  | PreInc of access                 (* C/C++/Java/C# ++i or ++a[e] CHANGED *)
  | PreDec of access                 (* C/C++/Java/C# --i or --a[e] CHANGED *)
````
then change eval to the following:
```Fsharp
| PreInc(acc) ->                                                (* CHANGED *)
        let (loc, store1) = (access acc locEnv gloEnv store)
        let newValue = (getSto store1 loc) + 1
        (newValue, setSto store1 loc newValue)
    | PreDec(acc) ->                                            (* CHANGED *)
        let (loc, store1) = (access acc locEnv gloEnv store)
        let newValue = (getSto store1 loc) + 1
        (newValue, setSto store1 loc newValue)
```
the idea is to first find the addresse of ``acc`` in ``store``.
Then find the value of ``acc`` in ``store`` using getSto,
and increase/decrease it by one. 
lastly add the new updated value to the ``store``.
# Exercise 7.5
no new tokens to be added. plus and minus tokens already exists. We only need to add this change to the parser:
````fsharp
ExprNotAccess:
  ...
  | PLUS PLUS Expr                      { PreInc($3) }          /* CHANGED */
  | MINUS MINUS Expr                    { PreInc($3) }          /* CHANGED */
;
````
here is a simply testcase to ensure it works:
````fsharp
void main()
{
    int i;
    i = 0;
    while (i < 10)
    {
        print ++i;
    }
    println;
}
````
now lets build the project:
````fsharp
dotnet build parse.fsproj
dotnet fsi -r bin/Debug/net10.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs
````
and test that it works as intended:
````fsharp
> open ParseAndRun;;
> run (fromFile "test/test1.c")[];; 
1 2 3 4 5 6 7 8 9 10                                                                                                                                
val it: Interp.store = map [(0, 10)]
````
# exercise 8.1
