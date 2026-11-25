
class A extends Object {
  boolean b;

  boolean f() { 
    this.b = 1;
    return this.b;
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    boolean b;
    b = a.f();
  }
  
}