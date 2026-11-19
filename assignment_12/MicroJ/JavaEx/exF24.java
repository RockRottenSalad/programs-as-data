
// More specific signature does not exists.
// Rule: E-Invk

class A {
  void f(int i) {
    println(1);
  }
}

class Main {
  void main() {
    A a = new A();
    a.f(true);
  }
}  
