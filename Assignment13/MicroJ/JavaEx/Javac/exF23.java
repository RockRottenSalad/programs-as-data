
class A extends Object {
  void f() { 
    System.out.printf("%s %n", 1);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f();
    int b;
    b = 0;
    b.f();
  }
  
}