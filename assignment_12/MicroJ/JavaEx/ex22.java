
// Checks that null constant is valid argument to class type parameter.
// This fails on Javac, because main is static and therefore this not allowed.

class A { }
class Main {

  void f(A a) {
    print(1);
  }
  
  void main() {
    this.f(null);
  }
}
