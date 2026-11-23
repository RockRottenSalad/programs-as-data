// micro-J example demonstrating instance object creation.
// Used in exercise

class A {
  int i0;
  int i1;
  int i2;
  int i3;
  int i4;
}

class Main {
  void main() {
    A a1 = new A();
    A a2 = new A();

    println(a1, a2);
  }
}
