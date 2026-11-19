/*

  Demonstrates hiding af class fields.

  Similar to ex01.java except that the two fields now have different
  types.

  This is allowed.
*/

class A { int x; }
class B extends A { boolean x; }

class Main {
  void main() {
    B b = new B();
    b.x = true;
    A ba = b;
    ba.x = 42;

    println(ba.x, b.x); // 42 true
  }
}
  
