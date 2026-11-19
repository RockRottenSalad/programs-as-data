/*
  Hiding af class fields

  It is the compile time type that decides what field to access.
   
  The compile time type of ab is A and therefore its the field A_i used when accessing ab.i.
 */

class A { int i; }
class B extends A { int i; }

class Main {
  void main() {
    A a = new A();
    a.i = 42;
    B b = new B();
    b.i = 43;

    A ab = b;
    B bb = b;
    println(a.i, b.i, ab.i, bb.i);   // 42 43 0 43
  }
}
  
