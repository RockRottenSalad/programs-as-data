
class A extends Object {
  int r;

  int sqrt(int n) { 
    this.r = 0;
    while (((this.r * this.r) < n)) this.r = (this.r + 1);
    return this.r;
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    System.out.printf("%s %n", new A().sqrt(n));
    return;
  }
  }
}