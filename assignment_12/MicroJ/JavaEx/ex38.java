// micro-Java - optimization of andalso and orelse

class Main {
  void main(int n) {
    int y = 1889;
    while (y < n) {
      y = y + 1;
      if (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0)) 
        print(y);
    }
  }
}
