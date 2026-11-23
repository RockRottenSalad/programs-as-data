// micro-Java - tail calls

class A {
  int f(int n) {
    if (n>0) 
      return this.f(n-1);
    else 
      return 17;
  }
}
  
class Main {
  void main(int n) {
    println(new A().f(n));
  }
}
