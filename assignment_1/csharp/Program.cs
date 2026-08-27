
abstract class Expr {
    public abstract int Eval(List<(string, int)> env);
    public abstract Expr Simplify();
} 

class CstI : Expr {
    protected int i;
    public CstI(int i) {
        this.i = i;
    }
    public override int Eval(List<(string, int)> env) {
        return i;
    }

    public override Expr Simplify() {
        return this;
    }

    override public string ToString() {
        return i.ToString();
    }
}

class Var : Expr {
    protected string name;
    public Var(string name) {
        this.name = name;
    }
    public override int Eval(List<(string, int)> env) {
        return env.Find(kv => kv.Item1 == name).Item2;
    }

    public override Expr Simplify() {
        return this;
    }

    override public string ToString() {
        return name;
    }
}

abstract class Binop : Expr {
    protected Expr l, r;
    public Binop(Expr l, Expr r) {
        this.l = l; this.r = r;
    }

    protected string ToStringHelper(string op) {
        return "(" + l.ToString() + " " + op + " " + r.ToString() + ")";
    }
}


class Add : Binop {
    public Add(Expr l, Expr r) : base(l, r) {}

    public override int Eval(List<(string, int)> env) {
        return l.Eval(env) + r.Eval(env);
    }

    public override Expr Simplify() {
        Expr l_simp = l.Simplify();
        Expr r_simp = r.Simplify();

        var empty_env = new List<(string, int)>();

        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) + r_simp.Eval(empty_env));
        }

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

class Sub : Binop {
    public Sub(Expr l, Expr r) : base(l, r) {}

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

        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) - r_simp.Eval(empty_env));
        }

        if(r_simp is CstI) {
            if(r_simp.Eval(empty_env) == 0) {
                return l_simp;
            }
        }

        return new Sub(l_simp, r_simp);
    }
}

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

        if(l_simp is CstI && r_simp is CstI) {
            return new CstI(l_simp.Eval(empty_env) * r_simp.Eval(empty_env));
        }

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
