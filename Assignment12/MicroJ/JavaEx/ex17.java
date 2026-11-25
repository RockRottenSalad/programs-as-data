/*
  Demonstrate that you may have main method in several classes.
  But only main in the Main class can be entry to program.

*/

class A {
  void main(int arg) {
    println(arg);
  }
}

class Main {
  void main() {
    println(42);
  }
}
