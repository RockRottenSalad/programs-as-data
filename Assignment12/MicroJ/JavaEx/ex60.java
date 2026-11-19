// Tests on micro-Java implementation of print as expressions.
// Semantics differs from Java where print is a method invocation
// expression, see jls 25 §14.8.

class A { }

class Main {
  void main() {
    // Test that print leaves result type on stack.
    A a = println(new A()); // @...
    println(a);             // @...
    println(print());       // 1
    print(println());       // 1

    println(print() + print()); // 2
    println(println(1) + println(3)); // 1 3 4
  }
}
