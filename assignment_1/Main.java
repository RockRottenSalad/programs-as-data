import java.util.HashMap;
import java.util.Map;

public class Main{
    static Map<String, Integer> env = new HashMap<>();
    public static void main(String[] args) {

        Expr ex1 = new Add(new CstI(17), new Var("z"));
        Expr ex2 = new Var("z");
        Expr ex3 = new Mul(new Add(new CstI(0), new Var("a")), new Sub(new CstI(1), new CstI(1)));
        Expr ex4 = new Mul(new Mul(new CstI(2), new Var("a")), new Sub(new CstI(11), new CstI(1)));

        System.out.println(ex1.toString());
        System.out.println(ex2.toString());
        System.out.println(ex3.toString());
        System.out.println(ex4.toString());

        env.put("z", 15);
        env.put("a", 13);

        System.out.println(ex1.eval(env));
        System.out.println(ex2.eval(env));
        System.out.println(ex3.eval(env));
        System.out.println(ex4.eval(env));
    }
}

abstract class Expr {
    public abstract int eval(Map<String, Integer> env);
    public abstract Expr simplify(Map<String, Integer> env);
}

    class Var extends Expr {
        public String value;

        public Var(String v){
            this.value = v;
        }


        @Override
        public String toString() {
            return value;
        }

        @Override
        public int eval(Map<String, Integer> env) {
            return env.get(value);
            //this will cause uncaught exceptions
        }


        @Override
        public Expr simplify(Map<String, Integer> env) {
            return this;
        }
    }

    class CstI extends Expr {
        public Integer value;

        public CstI(int i){
            this.value = i;
        }

        @Override
        public String toString() {
            return value.toString();
        }

        @Override
        public int eval(Map<String, Integer> env) {
            return value;
        }

        @Override
        public Expr simplify(Map<String, Integer> env) {
            return this;
        }
    }

    abstract class Binop extends Expr {
        public String ope;
        public Expr e1;
        public Expr e2;

        public Binop(Expr e1, Expr e2){
            this.e1 = e1;
            this.e2 = e2;
            this.ope = " ? ";
        }

        @Override
        public String toString() {
            return "(" + e1.toString() + ope + e2.toString() + ")";
        }
    }
    class Sub extends Binop {
        public Sub(Expr e1, Expr e2) {
            super(e1, e2);
            this.ope = " - ";
        }

        @Override
        public int eval(Map<String, Integer> env) {
            return e1.eval(env) - e2.eval(env);
        }

        @Override
        public Expr simplify(Map<String, Integer> env) {
            Expr ex1 = e1.simplify(env);
            Expr ex2 = e2.simplify(env);
            if (ex2.eval(env) == 0) {
                return ex1;
            } else if (ex1.eval(env) == ex2.eval(env)){
                return new CstI(0);
            }
            return new Sub(ex1, ex2);
        }
    }

    class Add extends Binop {
        public Add(Expr e1, Expr e2) {
            super(e1, e2);
            this.ope = " + ";
        }
        @Override
        public int eval(Map<String, Integer> env) {
            return e1.eval(env) + e2.eval(env);
        }

        @Override
        public Expr simplify(Map<String, Integer> env) {
            Expr ex1 = e1.simplify(env);
            Expr ex2 = e2.simplify(env);
            if (ex1.eval(env) == 0) {
                return ex2;
            } else if (ex2.eval(env) == 0){
                return ex1;
            } else if (ex1.eval(env) == ex2.eval(env)){
                return new CstI(0);
            }
            return new Add(ex1, ex2);
        }
    }

    class Mul extends Binop {
        public Mul(Expr e1, Expr e2) {
            super(e1, e2);
            this.ope = " * ";
        }

        @Override
        public int eval(Map<String, Integer> env) {
            return e1.eval(env) * e2.eval(env);
        }

        @Override
        public Expr simplify(Map<String, Integer> env) {
            Expr ex1 = e1.simplify(env);
            Expr ex2 = e2.simplify(env);
            if (ex1.eval(env) == 1) {
                return ex2;
            } else if (ex2.eval(env) == 1){
                return ex1;
            } else if (ex1.eval(env) == 0){
                return new CstI(0);
            } else if (ex2.eval(env) == 0){
                return new CstI(0);
            }
            return new Mul(ex1, ex2);
        }
    }

