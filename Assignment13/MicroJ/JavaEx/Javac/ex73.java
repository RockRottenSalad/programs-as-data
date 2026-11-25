
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
    System.out.printf("%n");
  }

  Cons makelist(int n) { 
    Cons res;
    res = null;
    while ((n > 0)) { 
      res = this.cons(n,res);
      n = (n - 1);
    }
    return res;
  }

  int sumlist(Cons xs) { 
    int res;
    res = 0;
    while ((xs != null)) { 
      res = (res + xs.car);
      xs = xs.cdr;
    }
    return res;
  }

  Cons append(Cons xs,Cons ys) { 
    if ((xs != null)) return this.cons(xs.car,this.append(xs.cdr,ys)); else return ys;
  }

  Cons reverse(Cons xs) { 
    Cons res;
    res = null;
    while ((xs != null)) { 
      res = this.cons(xs.car,res);
      xs = xs.cdr;
    }
    return res;
  }

  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    Cons xs;
    xs = this.makelist(n);
    this.printlist(xs);
    this.printlist(this.reverse(xs));
    System.out.printf("%s %n", this.sumlist(xs));
    System.out.printf("%s %n", this.sumlist(this.append(xs,xs)));
  }
  }
}