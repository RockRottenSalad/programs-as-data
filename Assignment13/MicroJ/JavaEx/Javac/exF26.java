
class A extends Object {
  boolean f(Object o) { 
    System.out.printf("%s %n", 1);
    return true;
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    int i;
    i = a.f(new A());
  }
  
}