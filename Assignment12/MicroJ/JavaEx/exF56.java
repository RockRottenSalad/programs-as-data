// micro-Java - method must not be named super.

class A {
  void super() { println(2); }
}

class Main {
  void main() {
    new A().super();
  }
}
