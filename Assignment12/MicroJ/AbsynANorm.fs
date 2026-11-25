(* File microJ/Absyn.fs 
   Abstract syntax of a Java like language (micro-Java)
   sestoft@itu.dk, nh@itu.dk * 2025-05-18
*)

module Absyn

open Util

// Annotate line and column positions on AST nodes.
type pos = { line: int; column: int }

type constant =
    CstI of int                         // Integer constant             
  | CstS of string                      // String constant
  | CstB of bool                        // Boolean constant
  | CstN                                // The null object reference

type typ =
    TypI                                // Type int
  | TypS                                // Type string
  | TypB                                // Type bool  
  | TypO of string                      // Class type (class name)
  | TypN                                // Internal type null
  | TypV                                // Internal type void

type methodSignature = string * typ list   // A method signature is the methods name and parameter types.
type methodType = typ list * typ           // Parameter types and possibly return type. None used for void methods.
type classname = string                    // Class name represented as a string.

type expr =
    Access of access                                     // Variable or Field access
  | Assign of access * expr * pos                        // Assign variable or field
  | Cst of constant * typ option * pos                   // Constant                   
  | New of string * typ option * pos                     // Create instance of a class 
  | Prim1 of string * expr * typ option * pos            // Strict primitive operator                
  | Prim2 of string * expr * expr * typ option * pos     // Strict primitive operator                
  | PrimC of string * expr list * typ option * pos       // Primitive function call                  
  | Andalso of expr * expr * typ option * pos            // Sequential and                           
  | Orelse of expr * expr * typ option * pos             // Sequential or                            
  | Call of expr * string * methodSignature option * expr list * typ option * pos // Method invocation o.m(...)               

and access =
    AccVar of string * typ option * pos                  // Local variable access.
  | AccFld of expr * string * typ option * pos           // Class field access.

and stmt =
  | If of expr * stmt * stmt * pos            // Conditional                    
  | While of expr * stmt * pos                // While loop                     
  | Expr of expr                              // Expression (as in C or Java)
  | Return of expr option * pos               // Return from method             
  | Block of stmtordec list * pos             // Block: grouping and scope.

and stmtordec =
    Dec of typ * string * pos           // Declaration of local variable  
  | Stmt of stmt                        // A statement                    

and memberdec = 
    Methoddec of typ * string * paramdec list * stmt * pos  // Method: result type, name, parameters, body 
  | Fielddec of typ * string * pos                          // Field: type, field name 

and paramdec = typ * string

and classdec = 
    Classdec of classname * classname * memberdec list * pos      // Class: name, super and member declarations. 

and program  =
    Program of classdec list                      // Program: list of class declarations.

// All classes are represented by a tree with Object as the root node. 
// The type classHierarchy represents the hole program as a tree.      
// The function buildClassHierarchy builds the tree                     
type classHierarchy =
  Hierarchy of classdec * classHierarchy list   // Each class has a list of direct subclasses, possibly empty

// Helper functions
let emptyPos = { line=0; column=0 } // Empty position used for nodes added to the AST not being in source program.

let compileError pos errMsg = fatal ("Compile error on line " + (string)pos.line + ", column " +
                                     (string)pos.column + ": " + nl + "  " + errMsg)

let isField = function
    Methoddec _ -> false
  | Fielddec _ -> true

let isMethod = function
    Methoddec _ -> true
  | Fielddec _ -> false
  
let rec foldCH f e (Hierarchy(cd, chs)) =
  List.fold (foldCH f) (f e cd) chs
  
let mthSig = function
    Methoddec(t,m,pars,s,_) -> (m, List.map fst pars)
  | _ -> fatal "Absyn.mthSig: Got a field and not a method declaration."

let mthTyp = function
    Methoddec(t,m,pars,s,_) -> (List.map fst pars,t)
  | _ -> fatal "Absyn.mthTyp: Got a field and not a method declaration."

let mthResType = function
    Methoddec(t,m,pars,s,_) -> t
  | _ -> fatal "Absyn.mthResType: Got a field and not a method declaration."

let fldName = function
    Fielddec(t,f,_) -> f
  | _ -> fatal "Absyn.fldName: God a method and not a field declaration."

let fldTyp = function
    Fielddec(t,f,_) -> t
  | _ -> fatal "Absyn.fldTyp: God a method and not a field declaration."

let className (Classdec(n,_,_,_)) = n

// Find class in hierarchy.
// Slow, but convenient because no secondary mapping is needed - it works directly on the hierarchy.
let rec tryFindClass (Hierarchy(Classdec(n,s,_,_) as cd, chs)) cn =
  if cn = n then Some cd      
            else let rec loop = function
                   [] -> None
                 | ch::chs -> match tryFindClass ch cn with
                                None -> loop chs
                              | res -> res
                 loop chs

let findClass ch cn =
  match tryFindClass ch cn with
    Some cd -> cd
  | None -> fatal ("Absyn.findClass: Can't find class " + cn + ".")

let super (Classdec(_,n,_,_)) = n

// Get positions
let getPosAccess = function
    AccVar(_,_,pos) -> pos
  | AccFld(_,_,_,pos) -> pos

let getPosExpr = function
    Access a -> getPosAccess a
  | Assign(_,_,pos) -> pos
  | Cst(_,_,pos) -> pos
  | New(_,_,pos) -> pos
  | Prim1(_,_,_,pos) -> pos
  | Prim2(_,_,_,_,pos) -> pos
  | PrimC(_,_,_,pos) -> pos
  | Andalso(_,_,_,pos) -> pos
  | Orelse(_,_,_,pos) -> pos
  | Call(_,_,_,_,_,pos) -> pos

let getPosStmt = function
  | If(_,_,_,pos) -> pos
  | While(_,_,pos) -> pos
  | Expr e -> getPosExpr e
  | Return(_,pos) -> pos
  | Block(_,pos) -> pos

let getPosStmtOrDec = function
    Dec(_,_,pos) -> pos
  | Stmt s -> getPosStmt s

// Get type from access expression
let tryGetTypAccess = function
    AccVar(_,tOpt,_) -> tOpt
  | AccFld(_,_,tOpt,_) -> tOpt

// Get type from expression
let tryGetTypExpr = function  
    Access access -> tryGetTypAccess access
  | Assign (access,_,_) -> tryGetTypAccess access
  | Cst(_,tOpt,_) -> tOpt
  | New(_,tOpt,_) -> tOpt
  | Prim1(_,_,tOpt,_) -> tOpt
  | Prim2(_,_,_,tOpt,_) -> tOpt
  | PrimC(_,_,tOpt,_) -> tOpt
  | Andalso(_,_,tOpt,_) -> tOpt
  | Orelse(_,_,tOpt,_) -> tOpt
  | Call(_,_,_,_,tOpt,_) -> tOpt

let getTypExpr e =
  match tryGetTypExpr e with
    None -> failwith "Absyn.getTypExpr: can't get type from expression."
  | Some t -> t

let getClassTypExpr e =
  match getTypExpr e with
    TypO cn -> cn
  | _ -> failwith "Absyn.getClassTypExpr: failed"

// Pretty print abstract syntax

let ppConstant = function
    CstI i -> sprintf "%d" i
  | CstB b -> sprintf "%s" (if b then "true" else "false")
  | CstS s -> sprintf "\"%s\"" s
  | CstN   -> sprintf "null"

let ppTyp = function
    TypI   -> sprintf "int"
  | TypB   -> sprintf "boolean"
  | TypS   -> sprintf "string"
  | TypO o -> sprintf "%s" o
  | TypN   -> sprintf "null"
  | TypV   -> sprintf "void"

let ppTypOpt typP = function
    None -> ""
  | Some t -> if typP then "(" + (ppTyp t) + ")" else ""

let ppMthSig (n,typs) = "(" + n + ", (" + (String.concat " x " (List.map ppTyp typs)) + "))"

let ppMthSigOpt = function
    None -> ""
    // vSig, virtual signature, is the signature after overloading
    // resolution procedure, that exists in the virtual method table
  | Some vSig -> " vSig" + (ppMthSig vSig)

  
let ppMthTyp (typs,t) = "(" + (String.concat " x " (List.map ppTyp typs)) + ") -> " + (ppTyp t)

let ppParamDec (t,p) = ppTyp t + " " + p

let rec ppExpr javacP typP i e =
  let ppTypOpt = ppTypOpt typP
  let ppExpr = ppExpr javacP typP
  let ppJavac = ppJavac javacP typP
  let ppExprs = ppExprs javacP typP
  let ppAccess = ppAccess javacP typP
  match e with
    Access access -> ppAccess i access
  | Assign (access,e,_) -> "(" + (ppAccess i access) + " = " + (ppExpr i e) + ")"
  | Cst(c,tOpt,_) -> ppConstant c + (ppTypOpt tOpt)
  | New(cn,tOpt,_) -> "new " + cn + "()" + (ppTypOpt tOpt)
  | Prim1(ope,e,tOpt,_) -> ope + "(" + (ppExpr i e) + ")" + (ppTypOpt tOpt)
  | Prim2(ope,e1,e2,tOpt,_) -> "(" + (ppExpr i e1) + " " + ope + " " + (ppExpr i e2) + ")" + (ppTypOpt tOpt)
  | PrimC("print",args,tOpt,_) ->
    // The Javac commandline parameter changes pretty print for print
    // such that System.out.printf is used instead.
    if javacP
      then ppJavac i "" args
      else "print(" + (ppExprs i args) + ")" + (ppTypOpt tOpt)
  | PrimC("println",args,tOpt,_) ->
    if javacP
      then ppJavac i "%n" args
      else "println(" + (ppExprs i args) + ")" + (ppTypOpt tOpt)
  | PrimC(f,args,tOpt,_) -> f + "(" + (ppExprs i args) + ")" + (ppTypOpt tOpt)
  | Andalso(e1,e2,tOpt,_) -> ppExpr i e1 + "&&" + (ppExpr i e2) + (ppTypOpt tOpt)
  | Orelse(e1,e2,tOpt,_) -> ppExpr i e1 + "||" + (ppExpr i e2) + (ppTypOpt tOpt)
  | Call(e,m,vSigOpt,args,tOpt,_) ->
    (ppExpr i e) + "." + m + "(" + (ppExprs i args) + ")" + (ppMthSigOpt vSigOpt) + (ppTypOpt tOpt)

and ppAccess javacP typP i = function
    AccVar(v,tOpt,_)  -> v + (ppTypOpt typP tOpt)
  | AccFld(o,f,tOpt,_) -> ppExpr javacP typP i o + "." + f + (ppTypOpt typP tOpt)

and ppJavac javacP typP i nl = function
    [] -> "System.out.printf(\"" + nl + "\")"
  | args -> "System.out.printf(\"" + (String.replicate (List.length args) "%s ") + nl +
             "\", " + (ppExprs javacP typP i args) + ")"
        
and ppExprs javacP typP i es = String.concat "," (List.map (ppExpr javacP typP i) es)

let rec ppStmt javacP typP i s =
  let ppExpr = ppExpr javacP typP
  let ppAccess = ppAccess javacP typP  
  let ppStmt = ppStmt javacP typP
  match s with
  | If(e,s1,s2,_) -> "if (" + (ppExpr i e) + ") " + (ppStmt i s1) + " else " + (ppStmt i s2)
  | While(e,s,_) -> "while (" + (ppExpr i e) + ") " + (ppStmt i s)
    // System.out.printfn is allowed as statement expression in Java  
  | Expr(PrimC _ as e) when javacP -> ppExpr i e + ";"
    // Method invocation is allowed as statement expression in Java  
  | Expr(Call _ as e) when javacP -> ppExpr i e + ";"
    // Outer most parantheses in assignments not valid in Java, e.g. you can't write (j = 1); as a statement.
    // We apply same layout for Micro--Java.
  | Expr(Assign(a,e,_)) -> ppAccess i a + " = " + (ppExpr i e) + ";"     
  | Expr e when javacP ->
    // Need to implement assignment trick for expression statements in Java.      
    "var __foo_" + ((string)(newNum())) + " = " + (ppExpr i e) + ";"
  | Expr e -> ppExpr i e + ";"
  | Return(eOpt,_) -> match eOpt with
                        None -> "return;"
                      | Some e -> "return " + (ppExpr i e) + ";"
  | Block(stmtordecs,_) -> "{ " + nl +
                           (String.concat nl (List.map (ppStmtOrDec javacP typP (i+2)) stmtordecs)) + nl +
                           (indent i "}")

and ppStmtOrDec javacP typP i = function
    Dec(t,v,_) -> indent i (ppTyp t + " " + v + ";")
  | Stmt s -> indent i (ppStmt javacP typP i s)

let ppMemberDec javacP typP i md =
  let ppStmt = ppStmt javacP typP 
  match md with
    // The Javac commandline parameter changes pretty print for the
    // main function such that parameters to main are parsed and
    // bound to parameter variables in the method body.  We
    // introduce an extra block such that parameters are visibly
    // separated from method body.
    // Parameters are parsed according to type.
    // No extra block needed when there are no parameters.
    Methoddec(TypV,"main",[],s,_) when javacP ->
      nl + (indent i "") + "void main(String[] args) " + 
      (ppStmt i s) + nl + (indent i "")
  | Methoddec(TypV,"main",pars,s,_) when javacP ->
      let parseTypeJavac j (t,p) =
        match t with
          TypI -> "int " + p + " = " + "Integer.parseInt(args[" + (string j) + "]); "
        | TypS -> "String " + p + " = " + "args[" + (string j) + "]); " 
        | TypB -> "Boolean " + p + " = " + "Boolean.parseBoolean(args[" + (string j) + "]); "
        | _ -> CmdLine.panic (sprintf "Absyn.ppMemberDec in Javac mode - main parameter not supported: %A" t)
      nl + (indent i "") + "void main(String[] args) {" +   // Java SE 25 main method declaration.
      (Util.foldi (fun j acc par -> acc + nl + (indent (i+2) "") + (parseTypeJavac j par)) "" pars) +
      (ppStmt i s) + nl + (indent i "}")
  | Methoddec(t,m,pars,s,_) ->
      nl + (indent i "") +
      (ppTyp t) + " " + m + "(" + (String.concat "," (List.map ppParamDec pars)) + ") " + 
        (ppStmt i s)
  | Fielddec(t,f,_) -> nl + (indent i "") + ppTyp t + " " + f + ";"

let ppClassDec javacP typP i = function
    Classdec(c,extends,ms,_) ->
        nl + (indent i "") +
        ("class " + c + " extends " + extends + " {") + //nl +
            (String.concat nl (List.map (ppMemberDec javacP typP (i+2)) ms)) + nl +
            (indent i "") + "}"


// javacP: Code generated for Java SE 25, where main is allowed to be instance method.
let ppProg javacP typP p =
  let ppProgram i = function
      Program cds -> String.concat nl (List.map (ppClassDec javacP typP i) cds)
  ppProgram 0 p

let genJavac p =
  let path = (!CmdLine.globalCmdLine).source
  let (dir,filename) = (System.IO.Path.GetDirectoryName path, System.IO.Path.GetFileName path)
  let targetdir = System.IO.Path.Combine [|dir; "Javac"|]
  let target = System.IO.Path.Combine [|targetdir; filename|]  
  writeFile targetdir filename (ppProg true false p)
  sprintf "Generated javac program in file %s" target

// Only prints class names and relations
let ppClassHierarchy ch =
  let rec ppClassHierarchy' prefix (Hierarchy(Classdec(name,_,_,_),chs)) =
    let classStr = prefix + name
    match chs with
      [] -> [classStr]
      // Prefix should only be included in first child to increase readability
    | ch::chs -> ppClassHierarchy' (classStr + " <- ") ch @
                 (List.collect (ppClassHierarchy' (indent classStr.Length " <- ")) chs)

  String.concat nl (ppClassHierarchy' "" ch)

// Prettyprint class hierarchy
let ppCH typP ch =
  let doClass e cd = e + nl + (ppClassDec false typP 0 cd)
  foldCH doClass "" ch

// Build class hierarchy
// A list of class declarations are continuesly partitioned into those
// with certain super class until list is empty. We start with
// Object. The list is guaranteed to be empty as any class will end
// with Object up the chain.
let buildClassHierarchy (Program cds) =
  let rootObj = Classdec("Object","",[],emptyPos) // Empty Object class as root node

  let getClassdecsWithSuper superName cdsrest =
    List.partition (fun (Classdec(_,super,_,_)) -> superName = super) cdsrest

  let rec buildHierarchy (Classdec(superName,_,_,_) as cdSuper) cdsrest =
    let (childCds,cdsrest) = getClassdecsWithSuper superName cdsrest
    let (childHs,cdsrest) =
      List.foldBack (fun cd (childHs,cdsrest) ->
                     let (childH,cdsrest) = buildHierarchy cd cdsrest
                     (childH::childHs,cdsrest)) childCds ([],cdsrest)
    (Hierarchy(cdSuper, childHs), cdsrest)

  let ch =
    match buildHierarchy rootObj cds with
      (ch,[]) -> ch
    | (_,(Classdec(cn,supern,_,pos))::cdsrest) ->
        compileError pos  // Only show first class with missing super.
                     ("Class " + cn + " has non declared super class " + supern +
                      ", (buildHierarchy).")  // exF47.java

  CmdLine.debug (sprintf "Class hierarchy value: \n%A\n" ch);
  ch

// Return and Reachability Analysis
//   - check all non void methods complete abruptly
//   - check all statements are reachable.
//   - insert missing return statements on void methods.
let rraProg (p:program) : program =

  // Add return statement at end of outermost block.
  // The return statement has no position as it doesn't exist in source program.
  // An empty position is used.
  let addReturnStmt = function
      Block(stmtordecs,pos) -> Block (List.rev (Stmt (Return(None,emptyPos)) :: (List.rev stmtordecs)),pos)
    | s -> s

  let rec rraStmt = function
    | If(e,s1,s2,_) ->  rraStmt s1 || (rraStmt s2)
    | While(e,s,_) -> rraStmt s
    | Expr e -> true
    | Return(eOpt,_) -> false
    | Block([],_) -> true
    | Block(stmtordecs,pos) ->
      // Stop at first false in block, that is, stop at first return - what follows is dead code.
      let rec loop = function
          [] -> true
        | [sd] -> rraStmtOrDec sd
        | sd::sds -> 
            if rraStmtOrDec sd then
              loop sds
            else compileError (getPosStmtOrDec (List.head sds)) // List.head always succeeds
                              ("Statement or Declaration is unreachable, (rraStmt-Block).") // exF45.java, exF44.java, 
      loop stmtordecs

  and rraStmtOrDec = function
      Dec(t,v,_) -> true
    | Stmt s -> rraStmt s

  let rraMemberDec = function
      Methoddec(TypV,m,pars,s,pos) as mth ->
      if rraStmt s then
        Methoddec(TypV,m,pars,addReturnStmt s,pos)
      else mth
    | Methoddec(t,m,pars,s,pos) as mth ->
      if rraStmt s then
        compileError pos ("Non void method " + m +
                          " completes normally, (rraMemberDec).")
      else mth
    | Fielddec(t,f,_) as fld -> fld

  let rraClassDec = function
      Classdec(c,extends,ms,pos) ->
        Classdec(c,extends,List.foldBack (fun md ms -> rraMemberDec md :: ms) ms [],pos)

  let rraProgram = function
      Program cds -> Program (List.map rraClassDec cds)
      
  rraProgram p


// A-normalize form for assignments ae = e, where ae is an expression
// evaluating to a temporary instance object, see section and
// exercises on Garbage Collection.
// Do not distribute this function
let aNormClassDec cd =  

  // Name of dummy variables to use.
  let dummyVar = "_dummy-gc_"

  // Add dummy variable around assignment
  let addDummyAssign = function
      // A temporary object is created, ex61.java
      Expr(Assign(AccFld(New(cn, typcn, pos1),
                         x, typx, pos2),
                  e, pos3)) ->
        // Introduce temporary _dummy-gc_ variable with lifetime the statement only.
        Block([Dec(TypO cn, dummyVar, pos1);
               Stmt(Expr(Assign(AccVar(dummyVar, typcn, pos1),
                                New(cn, typcn, pos1), pos2)));
               Stmt(Expr(Assign(AccFld(Access(AccVar(dummyVar, typcn, pos2)),
                                       x, typx, pos2),
                                e, pos3)))],pos3)
      // This only works after type check, because the Call must have a return type.
      // Demonstrated on ex69.java
    | Expr(Assign(AccFld(Call(e1, f, mthSig, es, Some typR, pos1),
                         fld, fldTyp,pos2),
                  e2,pos3)) ->
        Block([Dec(typR, dummyVar, pos1);
               Stmt(Expr(Assign(AccVar(dummyVar, Some typR, pos1),
                                Call(e1, f, mthSig, es, Some typR, pos1), pos1)));
               Stmt(Expr(Assign(AccFld(Access(AccVar(dummyVar, Some typR, pos2)),
                                       fld, fldTyp, pos2),
                                e2, pos3)))],pos3)  
    | s -> s

  let rec aNormStmt = function
      Expr e -> addDummyAssign (Expr e)
    | If(e,s1,s2,pos) ->  If(e,aNormStmt s1, aNormStmt s2, pos)
    | While(e,s,pos) -> While(e,aNormStmt s, pos)
    | Return(eOpt,pos) -> Return(eOpt,pos)
    | Block(stmtordecs,pos) ->
      Block(List.foldBack (fun s C -> aNormStmtOrDec s :: C) stmtordecs [], pos)

  and aNormStmtOrDec = function
      Dec(t,v,pos) -> Dec(t,v,pos)
    | Stmt s -> Stmt (aNormStmt s)

  let aNormMemberDec = function
      Methoddec(t,m,pars,s,pos) as mth ->
        Methoddec(t,m,pars,aNormStmt s, pos)
    | Fielddec(t,f,pos) as fld -> fld

  match cd with
    Classdec(c,extends,ms,pos) ->
      Classdec(c,extends,List.foldBack (fun md ms -> aNormMemberDec md :: ms) ms [],pos)

let aNormGC_CH (ch:classHierarchy) : classHierarchy =
  let rec loop (Hierarchy(cd, chs)) =   // Type classes depth first order.
    let cd' = aNormClassDec cd
    let chs' = List.map loop chs
    Hierarchy(cd', chs')
  loop ch

