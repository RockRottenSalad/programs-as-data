// Return Analysis should work on if-then-else

class Main {
  int f(boolean b) {
    if (b) {
      println(1);
      return 1;
    } else {
      println(2);
      return 2;
    }
  }

  void main() {
    this.f(true); // 1
    this.f(true); // 2
  }
}

