class A {
  int f(int x) { return 1; }
  int f(boolean x) { return this.f(2) + 5; }
}

class B extends A {
  int f(boolean x) { return 3; }
  int f(int x) { return 4; }
  int fSuper(boolean x) { return super.f(x); }
  int fSuper2(boolean x) { return (new A()).f(true); }
}

class Main extends B {
  void main() {
    print(this.f(1));    // 1
    print(this.f(true)); // 3

    A a = new A();
    print(a.f(1));       // 1
    print(a.f(true));    // 2

    B b = new B();
    print(b.f(1));       // 1
    print(b.f(true));    // 3

    print(super.f(1));    // 1
    print(super.f(true)); // 3
    print(super.fSuper(true)); // 9
    print(super.fSuper2(true)); // 6
  }
}
