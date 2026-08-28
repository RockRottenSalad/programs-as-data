using System;
using System.Collections.Generic;

namespace Assignment1;

abstract class Environment
{
    public abstract int GetValue(string variable);
    public abstract void SetValue(string variable, int value);
}

class DictionaryEnvironment : Environment
{
    private Dictionary<string, int> _env = new();
    
    public override int GetValue(string variable)
    {
        return _env.TryGetValue(variable, out var value) ? value : throw new Exception("Variable \"" + variable + "\" not found!");
    }

    public override void SetValue(string variable, int value)
    {
        _env[variable] = value;
    }
}

abstract class Expr
{
    public abstract int Eval(Environment env);
    public abstract override string ToString();
}


abstract class Binop(Expr e1, Expr e2) : Expr
{
    protected abstract string Operator { get; }

    protected abstract int Compute(int lhs, int rhs);

    public override int Eval(Environment env)
    {
        return Compute(e1.Eval(env), e2.Eval(env));
    }
    
    public override string ToString()
    {
        return "(" + e1 + " " + Operator + " " + e2 + ")";
    }
    
}

class Add(Expr e1, Expr e2) : Binop(e1, e2)
{
    protected override string Operator => "+";
    protected override int Compute(int lhs, int rhs)
    {
        return lhs + rhs;
    }
}

class Mul(Expr e1, Expr e2) : Binop(e1, e2)
{
    protected override string Operator => "*";
    protected override int Compute(int lhs, int rhs)
    {
        return lhs * rhs;
    }
}

class Sub(Expr e1, Expr e2) : Binop(e1, e2)
{
    protected override string Operator => "-";
    protected override int Compute(int lhs, int rhs)
    {
        return lhs - rhs;
    }
    
}

class CstI(int x) : Expr
{
    public override int Eval(Environment env)
    {
        return x;
    }

    public override string ToString()
    {
        return "" + x;
    }
    
}

class Var(string x) : Expr
{
    public override int Eval(Environment env)
    {
        return env.GetValue(x);
    }

    public override string ToString()
    {
        return x;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Environment env = new DictionaryEnvironment();
        env.SetValue("a", 4);
        env.SetValue("b", 9);
        env.SetValue("z", -1);
        
        
        Expr e = new Add(new CstI(17), new Var("z"));
        Expr e2 = new Mul(new Var("a"), new Add(new CstI(17), new Var("z")));
        Expr e3 = new Add(new Var("a"), new Sub(new CstI(17), new Mul(new Var("z"), new Var("b"))));
        Expr e4 = new Mul(new Var("a"), new Add(new CstI(17), new Mul(new CstI(3), new Sub(new Var("a"), new CstI(2)))));
        Console.WriteLine(e.ToString());
        Console.WriteLine(e2.ToString());
        Console.WriteLine(e3.ToString());
        Console.WriteLine(e4.ToString());
        
        Console.WriteLine(e.Eval(env));
        Console.WriteLine(e2.Eval(env));
        Console.WriteLine(e3.Eval(env));
        Console.WriteLine(e4.Eval(env));
    }
}