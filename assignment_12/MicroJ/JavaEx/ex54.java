// micro-Java - print class values.

class A { }
class Main {
  void main() {
    A a = null;
    println(a); // @null

    a = new A();
    println(a); // @address

    println(null); // @null
  }
}
