
class A extends Object {
  int i1;

  int i2;

  int f() { 
    return (this.i1 + this.i2);
  }
}

class B extends A {
  int i2;

  int f() { 
    return this.i2;
  }

  int fSuper() { 
    return super.f();
  }

  int i2Super() { 
    return super.i2;
  }

  int g() { 
    return (this.i1 + this.i2);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.i1 = 1;
    a.i2 = 2;
    B b;
    b = new B();
    b.i1 = 4;
    b.i2 = 42;
    A ab;
    ab = b;
    System.out.printf("%s %s %s %n", a.i1,a.i2,a.f());
    System.out.printf("%s %s %s %s %s %s %n", b.i1,b.i2,b.f(),b.i2Super(),b.fSuper(),b.g());
    System.out.printf("%s %s %s %n", ab.i1,ab.i2,ab.f());
  }
  
}