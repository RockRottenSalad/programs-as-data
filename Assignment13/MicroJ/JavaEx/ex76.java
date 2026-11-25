// Similar to ListC example ex36.lc 

// We create a cyclic structure by creating an object with two fields all
// pointing back the the object itself.

// We provoke a garbage collection because a faulty collector may go
// into an infinite loop either when marking or sweeping or copying 
// the cyclic structure.

// Use n around 400000 to force GC.

class A {
  A a1;
  A a2;
}

class Main {

  void makegarbage(int n) {
    A a;
    int i = 0;
    while (i<n) {
      a = new A();
      i = i + 1;
    }
  }

  void main(int n) {
    A a = new A();
    a.a1 = a;
    a.a2 = a.a1;
    println(a, a.a1, a.a2);  // All same addresses
    this.makegarbage(n);
    println(a, a.a1, a.a2);  // All same addresses as above.
  }

}
