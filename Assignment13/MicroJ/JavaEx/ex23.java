// Example of allowed override because return type of f_B is a
// subtype of return type of f_A.


class C { }
class D extends C { }
class A {
  C f(int a) {
    print(1);
    return new C();
  }
}

class B extends A {
  D f(int b) {
    print(2);
    return new D();
  }
}


class Main {  
  void main() {
    A ba = new B();
    ba.f(1);   // 2
    ba.f(1);   // 2 

    B bb = new B();
    bb.f(1);   // 2
    bb.f(1);   // 2
  }
}
