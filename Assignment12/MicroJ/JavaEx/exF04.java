// Should not type due to use of super as a parameter.
// Rule: MD-Method

class A {
  int f(int super) {
    return 2;
  }

}
