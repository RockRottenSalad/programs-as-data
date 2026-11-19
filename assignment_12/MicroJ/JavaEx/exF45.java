// It is a compile error, to have unreachable statements.

class Main {
  void f() {
    print(1);
    return;
    print(2);  // Unreachable
  }
  
  void main() {
    this.f();
  }
}

