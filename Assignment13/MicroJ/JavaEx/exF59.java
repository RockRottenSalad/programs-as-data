// Micro-java does not allow multiple inheritance.

// -genJavac will not generate Java SE 25 code as the program does not
// -parse.

class A { int i; }
class B { int i; }
class C extends A, B { int i; }

class Main {
  void main() {
    println(1);
  }
}
