// Can't apply super.super on field

class A { int i; } 

class B extends A { }

class Main extends B {
  void main() {
    print(super.super.i);
  }
}
