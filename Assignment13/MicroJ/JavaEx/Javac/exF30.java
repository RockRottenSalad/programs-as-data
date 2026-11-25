
class A extends Object {
  boolean b;

  boolean f() { 
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