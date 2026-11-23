/*
  Demonstrate that order of fields and functions in a class does not
  matter. Both fields and methods can be used in methods defined
  before they are defined.
*/

class A {
  int getI() {
    return this.i;
  }
  
  void setI(int i) {
    this.i = i;
  }
  
  int i;
}

class Main {
  void main() {
    A a = new A();
    a.setI(42);
    println (a.getI()); // 42
  }
}
