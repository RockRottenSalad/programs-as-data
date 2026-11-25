
class A extends Object {
  int f(int x) { 
    return 1;
  }

  int f(boolean x) { 
    return (this.f(2) + 5);
  }
}

class B extends A {
  int f(boolean x) { 
    return 3;
  }

  int f(int x) { 
    return 4;
  }

  int fSuper(boolean x) { 
    return super.f(x);
  }

  int fSuper2(boolean x) { 
    return new A().f(true);
  }
}

class Main extends B {
  void main(String[] args) { 
    System.out.printf("%s ", this.f(1));
    System.out.printf("%s ", this.f(true));
    A a;
    a = new A();
    System.out.printf("%s ", a.f(1));
    System.out.printf("%s ", a.f(true));
    B b;
    b = new B();
    System.out.printf("%s ", b.f(1));
    System.out.printf("%s ", b.f(true));
    System.out.printf("%s ", super.f(1));
    System.out.printf("%s ", super.f(true));
    System.out.printf("%s ", super.fSuper(true));
    System.out.printf("%s ", super.fSuper2(true));
  }
  
}