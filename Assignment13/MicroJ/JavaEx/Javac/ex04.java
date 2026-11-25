
class A extends Object {
  int x;
}

class B extends A {
  boolean x;
}

class Main extends Object {
  void main(String[] args) { 
    B b;
    b = new B();
    b.x = true;
    A ba;
    ba = b;
    ba.x = 42;
    System.out.printf("%s %s %n", ba.x,b.x);
  }
  
}