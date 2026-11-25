// Micro-java does not allow multiple inheritance.
// Diamond structure

// -genJavac will not generate Java SE 25 code as the program does not
// -parse.

class A { int i; }
class B extends A { int i; }
class C extends A { int i; }
class D extends B, C { int i; }

class Main {
  void main() {
    println(1);
  }
}
