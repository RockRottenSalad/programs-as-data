
// Method signatures are not distinct.
// Rule: CT-Class

class A {
  int f(int i, boolean b) { return i; }
  boolean f(int i, boolean b) { return b; }  
}
