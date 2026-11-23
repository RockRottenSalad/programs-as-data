// Minimal example of virtual and non-virtual method invocation.

class A {
  int i;
  void f(int j) {
    this.i = 1 + j;
    println(this.i);
  }
}

class Main extends A {
  void main() {
    A a = new A();
    a.f(1);       // 2
    super.f(2);   // 3
  }
}
