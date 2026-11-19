
// More specific signature does not exists.
// Rule: E-Invk

class A {
  void f(A a, Object o) {
    println(1);
  }

  void f(Object o, A a) {
    println(2);
  }

}

class Main {
  void main() {
    A a = new A();
    a.f(new A(), new A());
  }
}  
