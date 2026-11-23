// micro-Java -- tail calls

class A {
  void f(int n) {
    if (n!=0) {
      print(n);
      this.f(n-1);
    } else 
      print(999999);
  }
}

class Main {
  void main(int n) {
    new A().f(n);
  }
}
