
class B extends Foo {
  void f() { 
    System.out.printf("%s ", 1);
  }
}

class Main extends Object {
  void main(String[] args) { 
    B b;
    b = new B();
    b.f();
  }
  
}