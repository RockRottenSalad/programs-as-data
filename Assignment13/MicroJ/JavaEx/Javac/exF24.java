
class A extends Object {
  void f(int i) { 
    System.out.printf("%s %n", 1);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f(true);
  }
  
}