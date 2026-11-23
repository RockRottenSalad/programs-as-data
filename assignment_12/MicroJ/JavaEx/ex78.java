// Example creating temporary object in access expression.
// Used in exercise to make GC safe.
// See also ex61.java, ex69.java

class A {
  A a;
}  

class Main {
  A mkA() {
    return new A();
  }
  
  void main() {
    A a = new A();
    a.a = new A();
    this.mkA().a = new A();
  }
}


