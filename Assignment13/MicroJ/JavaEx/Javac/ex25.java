
class C extends Object {
}

class D extends C {
}

class A extends Object {
  C f(C c) { 
    System.out.printf("%s ", 1);
    return new C();
  }
}

class B extends A {
  D f(C c) { 
    System.out.printf("%s ", 2);
    return new D();
  }
}

class Main extends Object {
  void main(String[] args) { 
    A ba;
    ba = new B();
    ba.f(new C());
    ba.f(new D());
    B bb;
    bb = new B();
    bb.f(new C());
    bb.f(new D());
  }
  
}