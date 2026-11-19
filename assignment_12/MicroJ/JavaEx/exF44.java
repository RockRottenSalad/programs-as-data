// Definitive Return Analysis
// Return missing in else branch

class Main {
  int f(boolean b) {
    if (b) {
      print(1);
      return 1;
    } else {
      print(2);
    }
  }
  
  void main() {
    this.f(true);
  }
}
