// Example of allowed override because return type of f_B is a
// subtype of return type of f_A.


class C { }
class D extends C { }
class A {
  C f(C c) {
    print(1);
    return new C();
  }
}

class B extends A {
  D f(C c) {
    print(2);
    return new D();
  }
}


class Main {  
  void main() {
    A ba = new B();
    ba.f(new C());   // 2
    ba.f(new D());   // 2 

    B bb = new B();
    bb.f(new C());   // 2
    bb.f(new D());   // 2
  }
}
