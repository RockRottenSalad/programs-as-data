
class C extends Object {
  int g() { 
    return 0;
  }
}

class D extends C {
  int g() { 
    return 1;
  }
}

class A0 extends Object {
  C h(C c) { 
    return new C();
  }
}

class A extends A0 {
  int f(int x) { 
    return (x + x);
  }

  int g(int x) { 
    return (x + x);
  }

  D h(C c) { 
    return new D();
  }
}

class B extends A {
  int g(int x) { 
    return (x * 3);
  }
}

class Main extends Object {
  void main(String[] args) { 
    B b;
    b = new B();
    System.out.printf("%s %n", b.f(2));
    System.out.printf("%s %n", b.g(3));
    C c;
    c = b.h(new C());
    System.out.printf("%s %n", c.g());
  }
  
}