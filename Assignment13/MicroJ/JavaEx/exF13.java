
// Refers not declared field
// Rule: E-Field

class A {
  int i;
  void f() {
    this.j = 1;  // Type error, j not a field
  }
}
