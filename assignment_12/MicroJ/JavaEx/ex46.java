// micro-Java
// Field and local variables; function call in expression

class Main {
  int x;

  void main() {
    this.x = 111;
    print (222 + this.g(333));
  }

  int g(int y) {
    return this.x + y;
  }
}
