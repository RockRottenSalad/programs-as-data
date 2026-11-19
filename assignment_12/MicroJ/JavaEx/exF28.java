
// The field does not exists
// Rule: S-FieldAssign

class A {
  boolean b;
  boolean f() {
    this.b = 1;
    return this.b;
  }
}

class Main {
  void main() {
    A a = new A();
    boolean b = a.f();
  }
}  
