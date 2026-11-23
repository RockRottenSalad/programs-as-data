/*
  Method returning object.
 */

class A {
  int x;
}

class B extends A {
  int x;
}

class DoIt {
  A getA() {
    A a = new A();
    a.x = 1;
    return a;
  }

  B getB() {
    B b = new B();
    b.x = 2;
    return b;
  }
  
}

class Main {
  void main() {
    DoIt doit = new DoIt();
    
    println(doit.getA().x, doit.getB().x); // 1 2
  }
}
