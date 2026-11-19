// All fields must be accessed using this.

class Main {
  int i;
  void main() {
    { int i = 1; print(i); }
    this.i = 2;
    i = 3;    // Type error, because i is a field and must be reference using this.
    print(i);
  }
}
