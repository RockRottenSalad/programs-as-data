
class A extends Object {
  int f() { 
    return 1;
  }
}

class B extends A {
  int f() { 
    return 2;
  }
}

class C extends B {
}

class D extends C {
  int f() { 
    return 3;
  }
}

class E extends D {
}

class F extends E {
}

class G extends F {
  int f() { 
    return 4;
  }
}

class H extends G {
}

class Main extends Object {
  void main(String[] args) { 
    A a;
    a = new A();
    B b;
    b = new B();
    C c;
    c = new C();
    D d;
    d = new D();
    E e;
    e = new E();
    F f;
    f = new F();
    G g;
    g = new G();
    H h;
    h = new H();
    System.out.printf("%s %s %s %s %s %s %s %s %n", a.f(),b.f(),c.f(),d.f(),e.f(),f.f(),g.f(),h.f());
    A ab;
    ab = b;
    A ac;
    ac = c;
    B bc;
    bc = c;
    System.out.printf("%s %s %s %n", ab.f(),ac.f(),bc.f());
    A ad;
    ad = d;
    B bd;
    bd = d;
    C cd;
    cd = d;
    System.out.printf("%s %s %s %n", ad.f(),bd.f(),cd.f());
    A ah;
    ah = h;
    B bh;
    bh = h;
    C ch;
    ch = h;
    D dh;
    dh = h;
    E eh;
    eh = h;
    F fh;
    fh = h;
    G gh;
    gh = h;
    System.out.printf("%s %s %s %s %s %s %s %n", ah.f(),bh.f(),ch.f(),dh.f(),eh.f(),fh.f(),gh.f());
  }
  
}