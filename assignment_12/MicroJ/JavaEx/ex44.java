// micro-Java- composite conditions in different contexts

class Main {
  void main(int m, int n) {
    boolean b;
    if (m == 0 && n == 0)
      print(1111);
    else
      print(2222);
    b = (m == 0 && n == 0);
    print(b);
  }
}
