
class A extends Object {
  int i;
}

class B extends A {
  int i;
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.i = 42;
    B b;
    b = new B();
    b.i = 43;
    A ab;
    ab = b;
    B bb;
    bb = b;
    System.out.printf("%s %s %s %s %n", a.i,b.i,ab.i,bb.i);
  }
  
}