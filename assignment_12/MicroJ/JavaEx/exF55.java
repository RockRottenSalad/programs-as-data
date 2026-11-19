// micro-Java - method must not be named this.

class A {
  void this() { println(2); }
}

class Main {
  void main() {
    new A().this();
  }
}
