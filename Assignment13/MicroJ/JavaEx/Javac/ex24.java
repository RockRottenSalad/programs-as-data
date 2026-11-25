
class C extends Object {
}

class D extends C {
}

class A extends Object {
  C f(C c) { 
    System.out.printf("%s %n", 1);
    return new C();
  }

  C g(C c) { 
    System.out.printf("%s %n", 2);
    return new C();
  }
}

class B extends A {
  D f(D d) { 
    System.out.printf("%s %n", 3);
    return new D();
  }

  C g(C c) { 
    System.out.printf("%s %n", 4);
    return new C();
  }

  D g(D d) { 
    System.out.printf("%s %n", 5);
    return new D();
  }
}

class Main extends Object {
  void main(String[] args) { 
    B b;
    b = new B();
    b.f(new D());
    b.f(new C());
    b.g(new C());
    b.g(new D());
    A ba;
    ba = new B();
    ba.f(new D());
    ba.f(new C());
    ba.g(new C());
    ba.g(new D());
  }
  
}