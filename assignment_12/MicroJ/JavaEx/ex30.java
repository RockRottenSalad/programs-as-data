// The null constant can be assigned to fields, parameters and variables.


class A {
  A a;
  void f(A a) {
    println(1);
  }

  A g(A a) {
    this.a = null;
    println(2);
    return a;
  }
    
}
class Main {
  void main() {
    A a = null;
    A a2 = new A();
    a2.f(a);         // 1
    a2.f(null);      // 1
    a2.g(a);         // 2
    a2.g(null);      // 2
  }
}

