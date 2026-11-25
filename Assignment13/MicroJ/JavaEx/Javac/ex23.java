
class C extends Object {
}

class D extends C {
}

class A extends Object {
  C f(int a) { 
    System.out.printf("%s ", 1);
    return new C();
  }
}

class B extends A {
  D f(int b) { 
    System.out.printf("%s ", 2);
    return new D();
  }
}

class Main extends Object {
  void main(String[] args) { 
    A ba;
    ba = new B();
    ba.f(1);
    ba.f(1);
    B bb;
    bb = new B();
    bb.f(1);
    bb.f(1);
  }
  
}