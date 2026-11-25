// Return with value only allowed in non void methods.
// Rule: S-ReturnVal

class A {
  boolean b;
  void f() {
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
