
class A extends Object {
  boolean leapyear(int y) { 
    return ((y % 4) == 0)&&((y % 100) != 0)||((y % 400) == 0);
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    A a;
    a = new A();
    int y;
    y = 1889;
    while ((y < n)) { 
      y = (y + 1);
      if (a.leapyear(y)) System.out.printf("%s ", y); else { 

      }
    }
  }
  }
}