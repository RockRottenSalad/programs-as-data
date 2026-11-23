
// The field does not exists
// Rule: S-FieldAssign

class A {
  boolean b;
  boolean f() {
    this.a = true;
    return this.b;
  }
}

class Main {
  void main() {
    A a = new A();
    boolean b = a.f();
  }
}  
