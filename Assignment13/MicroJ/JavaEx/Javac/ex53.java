
class A extends Object {
  int i;

  int j;
}

class B extends A {
  int i;

  int k;

  void f() { 
    this.i = 42;
    this.k = 43;
    super.i = 44;
    super.j = 45;
    System.out.printf("%s %s %s %s %n", this.i,this.k,super.i,super.j);
  }
}

class Main extends Object {
  void main(String[] args) { 
    new B().f();
  }
  
}