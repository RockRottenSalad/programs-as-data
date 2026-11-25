
class Main extends Object {
  int f(int n) { 
    if ((n > 0)) return this.f((n - 1)); else return 17;
  }

  void main(String[] args) { 
    System.out.printf("%s ", this.f(10));
  }
  
}