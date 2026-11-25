
class S extends Object {
}

class T extends S {
}

class H extends T {
}

class U extends Object {
}

class V extends U {
}

class Main extends Object {
  T f(Object o,U u,S s) { 
    System.out.printf("%s %n", 1);
    return new T();
  }

  S f(U u,Object o,S s) { 
    System.out.printf("%s %n", 2);
    return new S();
  }

  S f(V v,Object o,T t) { 
    System.out.printf("%s %n", 3);
    return new S();
  }

  S f(V v1,V v2,H h) { 
    System.out.printf("%s %n", 4);
    return new S();
  }

  void main(String[] args) { 
    this.f(new U(),new Object(),new S());
    this.f(new U(),new Object(),new T());
    this.f(new V(),new V(),new H());
    this.f(null,null,null);
  }
  
}