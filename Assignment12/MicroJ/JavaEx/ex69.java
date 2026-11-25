// Relates to Section on garbage collection illustrating that it is
// unsound to have a temporary object created.
// See also ex61.java, ex78.java
// This program is extended with method returning object.

class A {
  A a;

  A m() { return new A(); }
}  

class Main {

  A m() {
    return new A();
  }
  
  void main() {
    A a = new A();
    a.a = new A();
    new A().a = new A();
    this.m().a = new A();
    new A().m().m().a = new A();
  }
}


