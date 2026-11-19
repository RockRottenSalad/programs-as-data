// Similar to ListC example ex32.lc

// For any input n, the program should be able to run provided the
// heap is big enough.  But when the mark phase of the garbage
// collector uses a recursive auxiliary function, that function may
// overflow the C stack.

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
    // Allocate a long list, that is, deep data structure
    Cons longlist = null;
    while (n>0) {
      longlist = this.cons(n,longlist);
      n = n - 1;
    }
    this.printlist(longlist);
    // Allocate more data to provoke a garbage collection
    while (true) { 
      Cons xs;
      xs = this.cons(42,null);
    }
  }
}
