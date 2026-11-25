
class A extends Object {
  A a;

  void f(A a) { 
    System.out.printf("%s %n", 1);
  }

  A g(A a) { 
    this.a = null;
    System.out.printf("%s %n", 2);
    return a;
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = null;
    A a2;
    a2 = new A();
    a2.f(a);
    a2.f(null);
    a2.g(a);
    a2.g(null);
  }
  
}