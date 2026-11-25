
// The result of of method call not as expected.
// Rule: S-VarAssign

class A {
  boolean f(Object o) {
    println(1);
    return true;
  }
}

class Main {
  void main() {
    A a = new A();
    int i = a.f(new A());
  }
}  
