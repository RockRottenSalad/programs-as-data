
class A extends Object {
  int i;
}

class B extends A {
  int i;
}

class C extends B {
}

class D extends C {
  int i;
}

class E extends D {
}

class F extends E {
}

class G extends F {
  int i;
}

class H extends G {
}

class I extends C {
  int i;
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    a.i = 1;
    B b;
    b = new B();
    b.i = 2;
    C c;
    c = new C();
    c.i = 3;
    D d;
    d = new D();
    d.i = 4;
    E e;
    e = new E();
    e.i = 5;
    F f;
    f = new F();
    f.i = 6;
    G g;
    g = new G();
    g.i = 7;
    H h;
    h = new H();
    h.i = 8;
    I i;
    i = new I();
    i.i = 9;
    System.out.printf("%s %s %s %s %s %s %s %s %s %n", a.i,b.i,c.i,d.i,e.i,f.i,g.i,h.i,i.i);
    a = h;
    a.i = 1;
    b = h;
    b.i = 2;
    c = h;
    d = h;
    d.i = 3;
    e = h;
    f = h;
    g = h;
    g.i = 4;
    System.out.printf("%s %s %s %s %s %s %s %s %n", a.i,b.i,c.i,d.i,e.i,f.i,g.i,h.i);
  }
  
}