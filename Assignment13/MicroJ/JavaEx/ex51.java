// Example ex51.java used in MicroJ exercise.


class S { }
class T extends S { }
class H extends T { }

class U {
  S f(S c) {
    println(1);
    return new S();
  }

  S g(T t) {
    println(2);
    return new S();
  }

  H h1(T t, Object o) {
    println(3);
    return new H();
  }

  H h2(Object o, T t) {
    println(4);
    return new H();
  }
}

class V extends U {
  T f(T t) {
    println(5);
    return new T();
  }

  S g(T t) {
    println(6);
    return new S();
  }

}

class Main {

  H f(T t) {
    println(7);
    return new H();
  }

  void main() {
    this.f(new H());   // 7
    this.f(new T());   // 7

    V v = new V();
    v.f(new S());      // 1
    v.f(new T());      // 5
    v.g(new T());      // 6

    U vu = v;
    vu.f(new S());     // 1
    vu.f(new T());     // 1
    vu.g(new T());     // 6
    vu.h1(new T(), new Object()); // 3
    vu.h1(new H(), new Object()); // 3
    vu.h2(new Object(), new T()); // 4
    
    U u = new U();
    u.f(new T());           // 1
    u.g(new H());           // 2
    u.h1(new T(), new H()); // 3
    u.h2(new S(), new T()); // 4
    
    
  }
}
