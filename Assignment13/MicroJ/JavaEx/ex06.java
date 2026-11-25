/*

  Call to super.m() is a static call to m(), and not part of dynamic
  dispatch. The method to call is resolved at compile time.

 */

class A {
  void m() { println(42); }
}

class B extends A {
  void m() {
    println(43);
    super.m();
  }
}

class C extends B {
  void m() {
    println(44);
    super.m();
  }
}

class Main {
  void main() {
    A a = new C();
    a.m(); // 44 43 42
  }
}
