
class A extends Object {
  int x;
}

class B extends A {
  int x;
}

class DoIt extends Object {
  A getA() { 
    A a;
    a = new A();
    a.x = 1;
    return a;
  }

  B getB() { 
    B b;
    b = new B();
    b.x = 2;
    return b;
  }
}

class Main extends Object {
  void main(String[] args) { 
    DoIt doit;
    doit = new DoIt();
    System.out.printf("%s %s %n", doit.getA().x,doit.getB().x);
  }
  
}