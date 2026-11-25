// Actual result type must be a subtype to expected result type of method.
// Rule: S-ReturnVal

class A {
  boolean b;
  boolean f() {
    this.b = true;
    return 1;
  }
}

class Main {
  void main() {
    A a = new A();
    a.f();
  }
}  
