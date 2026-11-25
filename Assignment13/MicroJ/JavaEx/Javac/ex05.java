
class A extends Object {
  void m() { 
    System.out.printf("%s %n", 42);
  }
}

class B extends A {
  void m() { 
    System.out.printf("%s %n", 43);
  }

  void superm() { 
    super.m();
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    B b;
    b = new B();
    A ba;
    ba = b;
    a.m();
    b.m();
    ba.m();
    b.superm();
  }
  
}