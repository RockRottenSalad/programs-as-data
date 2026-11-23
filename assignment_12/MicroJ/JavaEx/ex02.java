/*
  In class hierarchies you may redefine class fields and methods. This
  example illustrates the semantics of what methods and fields that
  are actually accesed and called?

  Class fields follows the compile time type of the object and find
  the fields closest up the chain starting with the compile time type
  (Hiding).

  For instance:
    b.i1 below refers to the field i1 in class A.
    b.i2Super() returns i2 in class A, that is zero - never initialized.
    b.fSuper() add i1 and i2 in class A. i2 is zero, why result is 4
    ab.i2 is field i2 in class A.
    
  For methods we always take the closest method up the chain with
  the outset in the runtime object type, Dynamic dispatch.

    ab.f() is method f in class B.
*/

class A {
  int i1;
  int i2;
  
  int f() { return this.i1 + this.i2; }
}

class B extends A {
  int i2;
  
  int f() { return this.i2; }
  int fSuper() { return super.f(); }
  int i2Super() { return super.i2; }
  int g() { return this.i1 + this.i2; }
}

class Main {
  void main() {
    A a = new A();
    a.i1 = 1;
    a.i2 = 2;
    B b = new B();
    b.i1 = 4;
    b.i2 = 42;
    A ab = b;

    println(a.i1, a.i2, a.f());                          // 1 2 3
    println(b.i1, b.i2, b.f(), b.i2Super(), b.fSuper(), b.g()); // 4 42 42 0 4 46
    println(ab.i1, ab.i2, ab.f());                       // 4 0 42
  }
}
