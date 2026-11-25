
class A extends Object {
  void f(int n) { 
    if ((n != 0)) { 
      System.out.printf("%s ", n);
      this.f((n - 1));
    } else System.out.printf("%s ", 999999);
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    new A().f(n);
  }
  }
}