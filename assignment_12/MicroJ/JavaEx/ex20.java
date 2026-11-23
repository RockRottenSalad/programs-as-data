/*
  Method Inheritance

  To inherit a method m_A from superclass A the following must be true:

  - The method m_A is member of A
  - No method m_B in B has method signature equal to m_A in A
  - No method m'_A overrides the method m_A in A.

  The last requirements is fulfilled by the type rule that no class
  must contain two methods with same signature.
  
*/

class C { int g() { return 0; } }
class D extends C { int g() { return 1; } }

class A0 {
  C h(C c) { return new C(); }
}

class A extends A0 {
  int f(int x) { return x + x; }
  int g(int x) { return x + x; }

  // Overriding h is ok, because D <: C, that is, return type is covariant.
  // Whereever the result of C.h can be used, the result of D.h can also be used.
  D h(C c) { return new D(); }
  
}

class B extends A {
  // f is inherited: f is member of A, no other method with same signature or override in A.
  int g(int x) { return x * 3; }   // g in class A is not inherited because g in B has same signature.
  // h is inherited: h is member of A. No other method in A overrides h in A. The method h in A0 is not in A.
}

class Main {
  void main() {
    B b = new B();
    println (b.f(2));   // 4
    println (b.g(3));   // 9
    C c = b.h(new C()); // Runtime type of c is D.
    println (c.g());    // 1
  }
}
