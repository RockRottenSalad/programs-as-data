// Verify type error having two methods in the same class with same
// method signature.
// Rule: CT-Class


class Main {
  int f(boolean b) {
    return 5;
  }

  boolean f(boolean b) {
    return true;
  }
  
  void main() {
    
  }
}
