
class B extends Object {
  int x;

  int m() { 
    return (this.x * 7);
  }
}

class C extends B {
  boolean x;
}

class Main extends Object {
  void main(String[] args) { 
    C c;
    c = new C();
    c.x = false;
    System.out.printf("%s %s %n", c.x,c.m());
  }
  
}