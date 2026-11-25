
class C extends Object {
}

class D extends C {
}

class A extends Object {
  C f(C c) { 
    System.out.printf("%s %n", 1);
    return new C();
  }

  boolean f(boolean x) { 
    System.out.printf("%s ", 6);
    return x;
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

  C superf(C c) { 
    System.out.printf("%s ", 7);
    return super.f(c);
  }

  C superf(D d) { 
    System.out.printf("%s ", 8);
    return super.f(d);
  }

  boolean superf(boolean x) { 
    System.out.printf("%s ", 9);
    return super.f(x);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f(new C());
    B b;
    b = new B();
    D d;
    d = b.f(new D());
    C c;
    c = b.f(new C());
    C c2;
    c2 = b.g(new C());
    D d2;
    d2 = b.g(new D());
    C c3;
    c3 = b.g(new D());
    A ba;
    ba = new B();
    C c4;
    c4 = ba.f(new D());
    C c5;
    c5 = ba.f(new C());
    C c6;
    c6 = ba.g(new C());
    C c7;
    c7 = ba.g(new D());
    Object o;
    o = ba.g(new D());
    System.out.printf("%s %n", b.f(true));
    b.superf(new C());
    b.superf(new D());
    System.out.printf("%s ", b.superf(false));
  }
  
}