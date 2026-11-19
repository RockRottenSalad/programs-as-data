// Allocates many objects, which immediately die and can be collected
// Similar to ex30.lc for ListC
// Apply argument of 100000 and GC will kick in.

class A {
  int i;
  int j;
}

class Main {
  void main(int i) {
    while (i>0) {
      A a = new A();
      i = i - 1;
    }
  }
}
