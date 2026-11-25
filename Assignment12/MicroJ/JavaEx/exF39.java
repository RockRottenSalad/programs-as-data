// Parameter names not distinct
// Rule: MD-Method

class A { }
class Main {
  A f(B b, int b) { return new A(); }
  
  void main() {
  }
}  
