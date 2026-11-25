// Example ex52.java used in MicroJ exercise.


class S { }
class T extends S { }
class H extends T { }
class U { }
class V extends U { }

class Main {

  T f(Object o, U u, S s) {
    println(1);
    return new T();
  }

  S f (U u, Object o, S s) {
    println(2);
    return new S();
  }

  S f(V v, Object o, T t) {
    println(3);
    return new S();
  }

  S f(V v1, V v2, H h) {
    println(4);
    return new S();
  }

  void main() {
    this.f(new U(), new Object(), new S());               // Most specific: U x Object x S
    //this.f(new U(), new U(), new S());                    // No - Object x U x S and U x Object x S both work technically.
    this.f(new U(), new Object(), new T());               // Most specific: U x Object x S
    this.f(new V(), new V(), new H());                    // Most specific: V x V x H
    this.f(null, null, null);                             // Most specific: V x V x H
    //this.f(new Object(), new Object(), new Object());     // No candidates
    //this.f(new Object(), new S(), new U());               // No candidates
  }
}
