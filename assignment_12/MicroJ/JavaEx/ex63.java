// micro-J example demonstrating tail calls.
// Similar to micro-C example ex12.c
// Used in exercise

class Main {
  int f(int n) {
    if (n>0) 
      return this.f(n-1);
    else 
      return 17;
  }

  void main() {
    print(this.f(10));
  }
}
