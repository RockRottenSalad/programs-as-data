// micro-Java - field r and sqrt

class A {
  int r;
  
  int sqrt(int n) {
    this.r = 0;
    while (this.r * this.r < n) 
      this.r = this.r + 1;
    return this.r;
    } 
}

class Main {
  void main(int n) {
    println (new A().sqrt(n));
    return;
  }
}
