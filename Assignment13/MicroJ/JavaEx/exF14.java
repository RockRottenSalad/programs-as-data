
// Refers not declared variable or parameter
// Rule: E-Var

class A {
  void f(int i) {
    j = 1;  // Type error, j not declared
  }
}
