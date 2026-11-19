// Two main methods exists
// Rule: Prog
// Java SE 25 allows many main methods and will compile below.

class A {
  void main() {
  }
}  

class Main extends A {
  // Two main methods exists as one is inherited from A.
  void main(int i) {
  }
}
