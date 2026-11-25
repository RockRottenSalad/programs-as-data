// Similar to example ex24.java with valid assignments 


class C { }
class D extends C { }

class A {
  C f(C c) {
    println(1);
    return new C();
  }

  // This method overloads above method, signatures (f,boolean) and
  // (f,C) are not override equivalent.
  boolean f(boolean x) {
    print(6);
    return x;
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

  // New overloaded method
  C superf(C c) {
    print(7);
    return super.f(c);  // Calls method with signature (f,C) in class A
  }

  // New overloaded method
  C superf(D d) {
    print(8);
    return super.f(d);  // Calls method with most specific signature (f,C) in class A
  }

  // New overloaded method
  boolean superf(boolean x) {
    print(9);
    return super.f(x);  // Calls method with signature (f,boolean) in class A.
  }
}


class Main {  
  void main() {
    A a = new A();
    a.f(new C());          // 1 Method f in class A is most specific overload
    B b = new B();
    D d = b.f(new D());    // 3 Method f in class B most specific overload.
    C c = b.f(new C());    // 1 Method f Inherited from class A most specific overload
    C c2 = b.g(new C());   // 4 Method g in class B overrides method g in class A
    D d2 = b.g(new D());   // 5 Method g in class B overloads the other method g in class B
    C c3 = b.g(new D());   // 5 Method g in class B overloads the other method g in class B    
                           //     Now upward cast of return type D to C.

    A ba = new B();
    C c4 = ba.f(new D());  // 1 Most specific method f in class A is f, and D <: C
    C c5 = ba.f(new C());  // 1 Perfect match on method f in class A.
    C c6 = ba.g(new C());  // 4 Perfect match on method g in class A, but dynamic dispats
                           //     calls the overrided version in class B.
    C c7 = ba.g(new D());  // 4 Most specific match on method g in class A, D <: C, and
                           //     dynamic dispatch calls the overrided version. The overloaded
                           //     D g(D d) does not exists in the compile time type of ba.

    Object o = ba.g(new D());  // 4 Now upward cast of return type C to Object.

    println(b.f(true));          // 6 true     (f,boolean) is inhertited in class B
    b.superf(new C());           // 7 1        (superf,C) is declared and overload in class B
    b.superf(new D());           // 8 1        (superf,D) is declared and overload in class B
    print(b.superf(false));      // 9 6 false  (superf,boolean) is declared and overload in class B
  }
}
