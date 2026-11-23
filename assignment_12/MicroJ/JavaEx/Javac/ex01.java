
class A extends Object {
  int i;
}

class B extends A {
  int i;
}

class Main extends Object {
  void main(String[] args) { 
    B b;
    b = new B();
    b.i = 43;
    A ba;
    ba = b;
    ba.i = 42;
    System.out.printf("%s %s %n", ba.i,b.i);
  }
  
}