// Non existing super

class B extends Foo {
  void f() { print(1); }
}  

class Main {
  void main() {
    B b = new B();
    b.f();
  }
}
