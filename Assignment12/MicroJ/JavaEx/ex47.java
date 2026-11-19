// micro-Java - Takeuchi function, in McCarthy's version, see
// https://en.wikipedia.org/wiki/Tak_(function)

class Main {
  int count; // Number of calls made to the tak function 

  void main() {
    this.count = 0;
    print(this.tak(10, 5, 0));  // Should print 5 and 10345
    print(this.count);
  }

  int tak(int x, int y, int z) {
    this.count = this.count + 1;
    if (y < x)
      return this.tak(this.tak(x - 1, y, z),
                      this.tak(y - 1, z, x),
                      this.tak(z - 1, x, y));
    else
      return z;
  }
}
