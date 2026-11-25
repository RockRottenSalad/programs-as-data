// Similar to ListC example ex34.lc

// Allocates a cons cell and accesses its components

class Cons {
  int car;
  int cdr;
}

class Main {

  Cons cons(int n, int i) {
    Cons c = new Cons();
    c.car = n;
    c.cdr = i;
    return c;
  }

  void main() {
    Cons c = this.cons(11, 15+18);
    println(c.car);
    println(c.cdr);
  }
}
