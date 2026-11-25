
class A extends Object {
  boolean b;

  void f() { 
    this.b = true;
    return 1;
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f();
  }
  
}