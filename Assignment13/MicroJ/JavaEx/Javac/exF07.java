
class C extends Object {
}

class D extends C {
}

class A extends Object {
  D f(int a) { 
    return new D();
  }
}

class B extends A {
  C f(int b) { 
    return new C();
  }
}

class Main extends Object {
  void main(String[] args) { 

  }
  
}