// Result type not void or declared.
// Rule: MD-Method

class A { }
class Main {
  B f() { return new A(); }
  
  void main() {
  }
}  
