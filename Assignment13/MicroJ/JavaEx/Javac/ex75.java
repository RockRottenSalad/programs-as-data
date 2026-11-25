
class IntCons extends Object {
  int car;

  int cdr;
}

class Cons extends Object {
  IntCons car;

  IntCons cdr;
}

class Main extends Object {
  void makegarbage(int n) { 
    IntCons c;
    int i;
    i = 0;
    while ((i < n)) { 
      c = new IntCons();
      i = (i + 1);
    }
  }

  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    IntCons v;
    v = new IntCons();
    v.car = 11;
    v.cdr = 22;
    Cons w;
    w = new Cons();
    w.car = v;
    w.cdr = v;
    System.out.printf("%s %s %n", w.car.car,w.car.cdr);
    System.out.printf("%s %s %n", w.cdr.car,w.cdr.cdr);
    v.car = 33;
    System.out.printf("%s %s %n", w.car.car,w.car.cdr);
    System.out.printf("%s %s %n", w.cdr.car,w.cdr.cdr);
    this.makegarbage(n);
    v.car = 44;
    System.out.printf("%s %s %n", w.car.car,w.car.cdr);
    System.out.printf("%s %s %n", w.cdr.car,w.cdr.cdr);
  }
  }
}