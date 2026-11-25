
class A extends Object {
  int getI() { 
    return this.i;
  }

  void setI(int i) { 
    this.i = i;
  }

  int i;
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.setI(42);
    System.out.printf("%s %n", a.getI());
  }
  
}