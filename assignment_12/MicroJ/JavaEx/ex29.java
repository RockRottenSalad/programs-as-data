// Only certain expressions can be used as expression statements in Java, jls25 §14.8.
// For instance, arithemic expressions are not allowed.
// Below work for micro-Java but not Java.
// Trick to assign variable instead.


class Main {
  void f() {
    print(1);
  }
  
  void main() {
    2+2;   // Not allowed for Javac. 
    (2+2); // Not allowed in Javac,
    this.f();
    println(1);
  }
}

