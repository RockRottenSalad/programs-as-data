
class A extends Object {
  int f(int i,boolean b) { 
    return i;
  }
}

class B extends A {
  boolean f(int i,boolean b) { 
    return b;
  }
}