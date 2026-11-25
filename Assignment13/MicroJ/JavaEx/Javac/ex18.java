
class A extends Object {
  int f() { 
    return 1;
  }

  boolean f(int i) { 
    return (i == 42);
  }
}

class B extends A {
  int g() { 
    return 2;
  }

  boolean f(int i) { 
    return (i == 45);
  }
}

class C extends A {
  int g() { 
    return 2;
  }

  int f() { 
    return 3;
  }
}

class D extends B {
  int f() { 
    return 4;
  }

  boolean f(int i) { 
    return (i == 47);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    System.out.printf("%s %s %s %n", a.f(),a.f(42),a.f(43));
    B b;
    b = new B();
    System.out.printf("%s %s %s %s %n", b.f(),b.g(),b.f(42),b.f(45));
    C c;
    c = new C();
    System.out.printf("%s %s %s %s %n", c.f(),c.g(),c.f(42),c.f(45));
    D d;
    d = new D();
    System.out.printf("%s %s %s %s %n", d.f(),d.g(),d.f(42),d.f(47));
  }
  
}