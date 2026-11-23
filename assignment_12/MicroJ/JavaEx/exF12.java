
// Method f_B can't override f_A because boolean is not a subtype of int
// Rule: CT-Class

class A {
  int f(int i, boolean b) { return i; }
}

class B extends A {
  boolean f(int i, boolean b) { return b; }  
}
