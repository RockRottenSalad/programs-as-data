/*
  Demonstrates hiding af class fields.

  It is the compile time type that decides what field to access.  The
  compile time type of ba is A and therefore its the field A_i used
  when accessing ba.i.
*/

class A { int i; }
class B extends A { int i; }

class Main {
  void main() {
    B b = new B();
    b.i = 43;
    A ba = b;
    ba.i = 42;

    println(ba.i, b.i); // 42 43
  }
}
  
