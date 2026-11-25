
class S extends Object {
}

class T extends S {
}

class H extends T {
}

class U extends Object {
  S f(S c) { 
    System.out.printf("%s %n", 1);
    return new S();
  }

  S g(T t) { 
    System.out.printf("%s %n", 2);
    return new S();
  }

  H h1(T t,Object o) { 
    System.out.printf("%s %n", 3);
    return new H();
  }

  H h2(Object o,T t) { 
    System.out.printf("%s %n", 4);
    return new H();
  }
}

class V extends U {
  T f(T t) { 
    System.out.printf("%s %n", 5);
    return new T();
  }

  S g(T t) { 
    System.out.printf("%s %n", 6);
    return new S();
  }
}

class Main extends Object {
  H f(T t) { 
    System.out.printf("%s %n", 7);
    return new H();
  }

  void main(String[] args) { 
    this.f(new H());
    this.f(new T());
    V v;
    v = new V();
    v.f(new S());
    v.f(new T());
    v.g(new T());
    U vu;
    vu = v;
    vu.f(new S());
    vu.f(new T());
    vu.g(new T());
    vu.h1(new T(),new Object());
    vu.h1(new H(),new Object());
    vu.h2(new Object(),new T());
    U u;
    u = new U();
    u.f(new T());
    u.g(new H());
    u.h1(new T(),new H());
    u.h2(new S(),new T());
  }
  
}