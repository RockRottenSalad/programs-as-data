
class Main extends Object {
  int x;

  void main(String[] args) { 
    this.x = 111;
    System.out.printf("%s ", (222 + this.g(333)));
  }
  

  int g(int y) { 
    return (this.x + y);
  }
}