// micro-Java
// Generating many objects and enforce garbage collection.

class A {
  int fld1;
  int fld2;
}

class Main {
  void main(int n) {
    A a;
    while (n>0) {
      a = new A();
      n = n - 1;
    }
  }
}
