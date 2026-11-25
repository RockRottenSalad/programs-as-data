// Heap allocated arrays in micro-Java.
// Used in exercise

class Main {
  void main() {
    int a[10];
    int n = 0;
    while (n<0) {
      a[n] = n;
      n = n + 1;
    }
    
    println(a);
  }
}
