// micro-Java - conjecture: seq will terminate for any n > 0

class Main {
  void main() {
    int k = 0;
    while (k < 10000) {
      k = k+1;
      if (this.seq(k) > 240) 
        print(k); 
    }
  }

  int seq(int i) {
    int count = 0;
    while (i != 1) {
      count = count + 1;
      if (i % 2 == 0) 
        i = i / 2;
      else
        i = i * 3 + 1;
    }
    return count;
  }
}
