// Allocates many cons objects but they do not become garbage
// Similar to ex31.lc for ListC
// Can run with n of 30000 without running out of memory.

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
  }
  
  void main(int n) {
    Cons xs = null;
    while (n>0) {
      xs = this.cons(n,xs);
      n = n - 1;
    }
    this.printlist(xs);
  }
}
