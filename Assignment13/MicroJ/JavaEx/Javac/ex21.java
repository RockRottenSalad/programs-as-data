
class A extends Object {
  int i;

  void doit(int i) { 
    System.out.printf("%s %n", i);
    this.i = 1;
    System.out.printf("%s %n", this.i);
    int k;
    k = 2;
    System.out.printf("%s %n", k);
    { 
      int l;
      l = 5;
      System.out.printf("%s %n", l);
    }
    { 
      int l;
      l = 6;
      System.out.printf("%s %n", l);
    }
  }
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.doit(42);
  }
  
}