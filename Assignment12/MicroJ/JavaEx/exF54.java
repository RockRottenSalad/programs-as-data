// micro-Java - class must not be named this.

class this { void Object() { println(1); } }

class Main {
  void main() {
    new this().Object();
  }
}
