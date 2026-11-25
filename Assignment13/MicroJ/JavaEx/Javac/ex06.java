
class A extends Object {
  void m() { 
    System.out.printf("%s %n", 42);
  }
}

class B extends A {
  void m() { 
    System.out.printf("%s %n", 43);
    super.m();
  }
}

class C extends B {
  void m() { 
    System.out.printf("%s %n", 44);
    super.m();
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new C();
    a.m();
  }
  
}