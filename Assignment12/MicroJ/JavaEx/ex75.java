// ex35.lc -- a listC source program 

// The v cons cell is shared between both car and cdr of w, 
// so an update to one should be visible from the other.

// We check that this holds also after provoking a garbage 
// collection, because a faulty (copying) collector may 
// unshare the car and cdr parts of w.

// To force GC use n around 400000.

class IntCons {
  int car;
  int cdr;
}

class Cons {
  IntCons car;
  IntCons cdr;
}

class Main {

  void makegarbage(int n) {
    IntCons c;
    int i = 0;
    while (i<n) {
      c = new IntCons();
      i = i + 1;
    }
  }
  
  void main(int n) {
    IntCons v = new IntCons();
    v.car = 11;
    v.cdr = 22;

    Cons w = new Cons();
    w.car = v;
    w.cdr = v;

    println(w.car.car, w.car.cdr);  // 11 22
    println(w.cdr.car, w.cdr.cdr);  // 11 22

    v.car = 33;

    println(w.car.car, w.car.cdr);  // 33 22
    println(w.cdr.car, w.cdr.cdr);  // 33 22

    this.makegarbage(n);
    v.car = 44;

    println(w.car.car, w.car.cdr);  // 44 22
    println(w.cdr.car, w.cdr.cdr);  // 44 22
  }

}
