// micro-J example demonstrating instance object creation.
// Used in exercise

class A {
  void f() {
    println(1);
  }
}

class Main {
  void main() {
    A a1 = new A();
    a1.f();         // 1
    a1 = null;
    println(a1);    // null
    a1.f();         // null pointer failure
  }
}
