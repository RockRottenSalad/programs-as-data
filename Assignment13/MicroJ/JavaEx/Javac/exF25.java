
class A extends Object {
  void f(A a,Object o) { 
    System.out.printf("%s %n", 1);
  }

  void f(Object o,A a) { 
    System.out.printf("%s %n", 2);
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.f(new A(),new A());
  }
  
}