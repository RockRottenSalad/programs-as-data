// micro-Java - update field in super class.

class A { int i; int j; }

class B extends A {
  int i;
  int k;
  
  void f() {
    this.i = 42;
    this.k = 43;
    super.i = 44;
    super.j = 45;
    println(this.i, this.k, super.i, super.j);  // 42 43 44 45
  }
}

class Main {
  void main() {
    new B().f();
  }
}
