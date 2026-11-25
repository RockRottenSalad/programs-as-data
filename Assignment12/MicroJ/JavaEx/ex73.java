// Similar to ListC example ex33.lc
// Various list-manipulating functions.
// No data to reclaim with GC and will run out of memory for a big enough n.
// Can run with n < 3000

class Cons {
  int car;
  Cons cdr;
}

class Main {

  Cons cons(int n, Cons xs) {
    Cons c = new Cons();
    c.car = n;
    c.cdr = xs;
    return c;
  }

  void printlist(Cons xs) {
    while (xs != null) {
      print(xs.car);
      xs = xs.cdr;
    }
    println();
  }
  
  Cons makelist(int n) {
    Cons res = null;
    while (n>0) {
      res = this.cons(n, res);
      n = n - 1;
    }
    return res;
  }

  int sumlist(Cons xs) {
    int res = 0;
    while (xs != null) {
      res = res + xs.car;
      xs = xs.cdr;
    }
    return res;
  }

  Cons append(Cons xs, Cons ys) {
    if (xs != null) 
      return this.cons(xs.car, this.append(xs.cdr, ys));
    else
      return ys;
  }

  Cons reverse(Cons xs) {
    Cons res = null;
    while (xs != null) {
      res = this.cons(xs.car, res);
      xs = xs.cdr;
    }
    return res;
  }
  
  void main(int n) {
    Cons xs;
    xs = this.makelist(n);
    this.printlist(xs);
    this.printlist(this.reverse(xs));
    println(this.sumlist(xs));
    println(this.sumlist(this.append(xs,xs)));
  }
}
