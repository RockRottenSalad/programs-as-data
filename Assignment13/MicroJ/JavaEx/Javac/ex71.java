
class Cons extends Object {
  int car;

  Cons cdr;
}

class Main extends Object {
  Cons cons(int n,Cons xs) { 
    Cons c;
    c = new Cons();
    c.car = n;
    c.cdr = xs;
    return c;
  }

  void printlist(Cons xs) { 
    while ((xs != null)) { 
      System.out.printf("%s ", xs.car);
      xs = xs.cdr;
    }
  }

  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    Cons xs;
    xs = null;
    while ((n > 0)) { 
      xs = this.cons(n,xs);
      n = (n - 1);
    }
    this.printlist(xs);
  }
  }
}