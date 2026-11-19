/*
  Variable Shadowing

  Variable shadowing is controlled by below rules:

  - It is not allowed to declare a variable with a name already in
  the set of method parameters.
  - It is allowed to declare a variable with a name already used as
  a field name in the class.
  - It is not allowed to declare a variable with a name that is
  already in scope.
  - It is allowed to have several variables declared
  with same name as long as their scopes do not overlap.

*/

class A {
  int i;
  void doit(int i) {
    println(i);      // 42
    this.i = 1;      // Parameter i shadows for field i - and need to access using this.
    println(this.i); // 1
    int k = 2;       // Allowed as not already declared.
    println(k);      // 2
    {
      //int i = 3;  // Not allowed, as i is method parameter.
      //int k = 4;  // Not allowed because k already in scope.
      int l = 5;  // Allowed as not already declared
      println(l); // 5
    }
    {
      int l = 6;  // Allowed because previous l not in scope.
      println(l); // 6
    }
  }
}

class Main {
  void main() {
    A a = new A();
    a.doit(42);
  }
}
