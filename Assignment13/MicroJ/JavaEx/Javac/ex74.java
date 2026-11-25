
class Cons extends Object {
  int car;

  int cdr;
}

class Main extends Object {
  Cons cons(int n,int i) { 
    Cons c;
    c = new Cons();
    c.car = n;
    c.cdr = i;
    return c;
  }

  void main(String[] args) { 
    Cons c;
    c = this.cons(11,(15 + 18));
    System.out.printf("%s %n", c.car);
    System.out.printf("%s %n", c.cdr);
  }
  
}