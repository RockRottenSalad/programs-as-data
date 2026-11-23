
// Class referenced not a class type.
// Rule: E-Field

class A {
  int i;
}

class B extends A {
  void f() {
    int c = 1;
    println(c.i);  // Type error, c not a class type
  }
}  
