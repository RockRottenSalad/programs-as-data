// Can't use return on non void method.
// Rule: S-Return

class A {
  boolean b;
  boolean f() {
    this.b = true;
    return;
  }
}

class Main {
  void main() {
    A a = new A();
    a.f();
  }
}  
