
class A extends Object {
}

class Main extends Object {
  void f(A a) { 
    System.out.printf("%s ", 1);
  }

  void main(String[] args) { 
    this.f(null);
  }
  
}