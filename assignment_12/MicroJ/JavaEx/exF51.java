// Can't apply super.super on method.

class A { void f() { print(1); } }

class B extends A { }

class Main extends B {
  void main() {
    super.super.f();
  }
}
