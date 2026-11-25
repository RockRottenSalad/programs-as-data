
class A extends Object {
  int f(int n) { 
    if ((n > 0)) return this.f((n - 1)); else return 17;
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    System.out.printf("%s %n", new A().f(n));
  }
  }
}