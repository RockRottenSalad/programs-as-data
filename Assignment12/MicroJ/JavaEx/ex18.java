/*
  Overriding, Dynamic Dispatch, vTable

  Bigger example

  In Java, only parameter list counts for overloading - not result type
 */

class A {
  int f() { return 1; }
  boolean f(int i) { return (i == 42); }
}

class B extends A {
  int g() { return 2; }
  boolean f(int i) { return (i == 45); }
}

class C extends A {
  int g() { return 2; }
  int f() { return 3; }
}

class D extends B {
  int f() { return 4; }
  boolean f(int i) { return (i == 47); }
}


class Main {

  void main() {
    A a = new A();
    println(a.f(), a.f(42), a.f(43)); // 1 true false

    B b = new B();
    println(b.f(), b.g(), b.f(42), b.f(45)); // 1 2 false true

    C c = new C();
    println(c.f(), c.g(), c.f(42), c.f(45)); // 3 2 true false

    D d = new D();
    println(d.f(), d.g(), d.f(42), d.f(47)); // 4 2 false true

  }
}
