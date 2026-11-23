// Example creating temporary object in access expression.
// Used in exercise to make GC safe.
// See also ex69.java, ex78.java

class A {
  A f;
}  

class Main {
  void main() {
    A a = new A();
    a.f = new A();
    new A().f = new A();
  }
}


