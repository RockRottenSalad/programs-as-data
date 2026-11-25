// Example combines inheritance, overriding and overloading

class C { }
class D extends C { }

class A {
  C f(C c) {
    println(1);
    return new C();
  }

  C g(C c) {
    println(2);
    return new C();
  }
}

class B extends A {
  // New method (f,D) in class B overloads the method (f,C) in class A.
  // Meaning class B inherits (f,C) from class A.
  // The two method signatures are not override-equivalent.
  D f(D d) {
    println(3);
    return new D();
  }

  // New method (g,C) overrides method (g,C) in class A
  C g(C c) {
    println(4);
    return new C();
  }

  // New method (g,D) overloads method (g,C) above.
  D g(D d) {
    println(5);
    return new D();
  }
}


class Main {  
  void main() {
    B b = new B();
    b.f(new D());   // 3 Method f in class B most specific overload.
    b.f(new C());   // 1 Method f Inherited from class A most specific overload
    b.g(new C());   // 4 Method g in class B overrides method g in class A
    b.g(new D());   // 5 Method g in class B overloads the other method g in class B
    
    A ba = new B();
    ba.f(new D());  // 1 Most specific method f in class A is f, and D <: C
    ba.f(new C());  // 1 Perfect match on method f in class A.
    ba.g(new C());  // 4 Perfect match on method g in class A, but dynamic dispats
                    //     calls the overrided version in class B.
    ba.g(new D());  // 4 Most specific match on method g in class A, D <: C, and
                    //     dynamic dispatch calls the overrided version. The overloaded
                    //     D g(D d) does not exists in the compile time type of ba.
    
  }
}
