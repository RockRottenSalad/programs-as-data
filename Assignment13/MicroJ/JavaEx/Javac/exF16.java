
class A extends Object {
  int i;
}

class B extends A {
  void f() { 
    int c;
    c = 1;
    c.i = 2;
  }
}