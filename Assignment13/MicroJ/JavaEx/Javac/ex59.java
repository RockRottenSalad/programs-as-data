
class A extends Object {
  int i;

  void f(int j) { 
    this.i = (1 + j);
    System.out.printf("%s %n", this.i);
  }
}

class Main extends A {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f(1);
    super.f(2);
  }
  
}