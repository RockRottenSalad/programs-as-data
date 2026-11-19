// micro-Java - leapyear, optimization of andalso and orelse

class A {
  boolean leapyear(int y) {
    return y % 4 == 0 && (y % 100 != 0 || y % 400 == 0);
  }
}

class Main {
  void main(int n) {
    A a = new A();
    int y = 1889;
    while (y < n) {
      y = y + 1;
      if (a.leapyear(y))
        print(y);
    }
  }

}
