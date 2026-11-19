// Example of not allowed override because return type of m_B is not a
// subtype of return type of m_A.
// Rule: CT-Class


class C { }
class D extends C { }
class A {
  D f(int a) {
    return new D();
  }
}

class B extends A {
  C f(int b) {
    return new C();
  }
}


class Main {  
  void main() {
    
  }
}
