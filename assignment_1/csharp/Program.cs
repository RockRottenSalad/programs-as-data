
// Represents all expressions
abstract class Expr {
    // Evaluate an expression within the given environment
    public abstract int Eval(List<(string, int)> env);

    // Recursively simplify the expression
    public abstract Expr Simplify();
} 

// Represents a constant integer
class CstI : Expr {
    protected int i;
    public CstI(int i) {
        this.i = i;
    }

    // To evaluate a constant, we just return it
    public override int Eval(List<(string, int)> env) {
        return i;
    }

    // Constant integer cannot be simplified
    public override Expr Simplify() {
        return this;
    }

    override public string ToString() {
        return i.ToString();
    }
}

// Represents a variable
class Var : Expr {
    protected string name;
    public Var(string name) {
        this.name = name;
    }

    // A variable can only be evaluated if it's defined within the environment
    // If the variable does not exist in the environment, an exception is thrown
    public override int Eval(List<(string, int)> env) {
        return env.Find(kv => kv.Item1 == name).Item2;
    }

    // A variable cannot be simplified any further
    public override Expr Simplify() {
        return this;
    }

    override public string ToString() {
        return name;
    }
}

// Used to represent binary operators such as '+', '-' etc...
abstract class Binop : Expr {
    protected Expr l, r;
    public Binop(Expr l, Expr r) {
        this.l = l; this.r = r;
    }

    // Wraps the expression in parenthesis
    protected string ToStringHelper(string op) {
        return "(" + l.ToString() + " " + op + " " + r.ToString() + ")";
    }
}


// Used to represent addition
class Add : Binop {
    public Add(Expr l, Expr r) : base(l, r) {}

    // Addition is defined and recursively evaluating both sides of the expression and adding them together
    public override int Eval(List<(string, int)> env) {
        return l.Eval(env) + r.Eval(env);
    }

    public override Expr Simplify() {
        Expr l_simp = l.Simplify();
        Expr r_simp = r.Simplify();

        var empty_env = new List<(string, int)>();

        // If both sides are constants, simply add them together
        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) + r_simp.Eval(empty_env));
        }

        // If either side is 0, return the other side
        if (l_simp is CstI) {
            if(l_simp.Eval(new List<(string, int)>()) == 0) {
                return r_simp;
            }
        } else if(r_simp is CstI) {
            if(r_simp.Eval(new List<(string, int)>()) == 0) {
                return l_simp;
            }
        }

        return new Add(l_simp, r_simp);
    }

    override public string ToString() {
        return ToStringHelper("+");
    }
}

// Used to represent subtraction
class Sub : Binop {
    public Sub(Expr l, Expr r) : base(l, r) {}

    // Subtraction is defined and recursively evaluating both sides of the expression and subtracting r from l 
    public override int Eval(List<(string, int)> env) {
        return l.Eval(env) - r.Eval(env);
    }

    override public string ToString() {
        return ToStringHelper("-");
    }

    public override Expr Simplify() {
        Expr l_simp = l.Simplify();
        Expr r_simp = r.Simplify();

        var empty_env = new List<(string, int)>();

        // If both sides are constants, simply evaluate
        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) - r_simp.Eval(empty_env));
        }

        // If the right side is 0, then return the left side
        if(r_simp is CstI) {
            if(r_simp.Eval(empty_env) == 0) {
                return l_simp;
            }
        }

        return new Sub(l_simp, r_simp);
    }
}

// Used to represent multiplication
class Mul : Binop {
    public Mul(Expr l, Expr r) : base(l, r) {}

    public override int Eval(List<(string, int)> env) {
        return l.Eval(env) * r.Eval(env);
    }

    override public string ToString() {
        return ToStringHelper("*");
    }

    public override Expr Simplify() {
        Expr l_simp = l.Simplify();
        Expr r_simp = r.Simplify();

        var empty_env = new List<(string, int)>();


        // If both sides are constants, simply multiply them
        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) * r_simp.Eval(empty_env));
        }

        // If one side is 1, return the other side
        // If 1 side is 0, return 0
        if (l_simp is CstI) {
            int l_eval = l_simp.Eval(empty_env);
            if(l_eval == 1) {
                return r_simp;
            } else if(l_eval == 0) {
                return new CstI(0);
            }
        } else if(r_simp is CstI) {
            int r_eval = r_simp.Eval(empty_env);
            if(r_eval == 1) {
                return l_simp;
            } else if(r_eval == 0) {
                return new CstI(0);
            }
        }

        return new Sub(l_simp, r_simp);
    }
}

public class EntryPoint {
    public static void Main() {
        Console.WriteLine("Testing ToString()");
        Expr e = new Add(new CstI(17), new Var("z"));
        Console.WriteLine(e.ToString());

        Expr e1 = new Mul(new Add(new CstI(5), new CstI(2)), new Var("z"));
        Console.WriteLine(e1.ToString());

        Expr e2 = new Sub(new Add(new Var("z"), new Var("w")), new Add(new Var("x"), new Var("y")));
        Console.WriteLine(e2.ToString());

        Expr e3 = new Add(new Mul(new CstI(9), new CstI(12)), new CstI(2));
        Console.WriteLine(e3.ToString());

        Console.WriteLine("\nTesting Simplify()");

        Expr e4 = new Add(new Mul(new CstI(1), new CstI(12)), new CstI(2));
        Console.WriteLine("Before simplify");
        Console.WriteLine(e4.ToString());
        Console.WriteLine("After simplify");
        Console.WriteLine(e4.Simplify().ToString());

        Expr e5 = new Add(new Mul(new CstI(1), new CstI(12)), new Var("x"));
        Console.WriteLine("Before simplify");
        Console.WriteLine(e5.ToString());
        Console.WriteLine("After simplify");
        Console.WriteLine(e5.Simplify().ToString());

        Expr e6 = new Add(new Var("x"), new Mul(new Var("y"), new CstI(0)));
        Console.WriteLine("Before simplify");
        Console.WriteLine(e6.ToString());
        Console.WriteLine("After simplify");
        Console.WriteLine(e6.Simplify().ToString());
    }
}
