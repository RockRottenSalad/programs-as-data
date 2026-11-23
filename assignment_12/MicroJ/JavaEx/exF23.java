
// Must be declared class type
// Rule: E-Invk

class A {
  void f() {
    println(1);
  }
}

class Main {
  void main() {
    A a = new A();
    a.f();

    int b = 0;
    b.f();
    
  }
}  
