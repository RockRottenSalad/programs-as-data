
class DoIt extends Object {
  int h;

  int m;

  int res;

  void doit(int arg1) { 
    int i;
    i = ((((33 * 7) + (8 / 4)) - (7 % (2 + 1))) + arg1);
    boolean b;
    b = !(false)&&true||(i == (2 + 1))&&(i != i)||(i < 2)||(i >= 2);
    System.out.printf("%s ", b);
    this.h = 23;
    this.m = 59;
    this.m3(6,18);
    this.res = this.m2();
    { 
      int res;
      res = 12345;
      System.out.printf("%s %n", res);
    }
    System.out.printf("%s %n", this.res);
    Time t1;
    t1 = new Time();
    t1.init(12,35);
    System.out.printf("%s %n", t1.getHours());
    System.out.printf("%s %n", t1.getMin());
  }

  int m2() { 
    boolean bFalse;
    bFalse = false;
    if (true) while (bFalse) if (true) return 111; else { 

    } else { 

    }
    if (true) while (bFalse) if (true) return 222; else return 333; else { 

    }
    if (true) while (bFalse) if (true) return 444; else return 555; else System.out.printf("%s ", -1);
    if (true) if (false) return 666; else return 777; else { 

    }
    return 999;
  }

  void m3(int x,int y) { 
    System.out.printf("%s ", this.h);
    System.out.printf("%s ", this.m);
    int i;
    i = x;
    while ((i <= y)) { 
      System.out.printf("%s ", i);
      i = (i + 1);
    }
    return;
  }
}

class Time extends Object {
  int m;

  void init(int h,int m) { 
    this.m = ((60 * h) + m);
  }

  int getHours() { 
    return (this.m / 60);
  }

  int getMin() { 
    return (this.m % 60);
  }

  void move(int m) { 
    this.m = (this.m + m);
  }
}

class Main extends Object {
  void main(String[] args) {
    int arg1 = Integer.parseInt(args[0]); { 
    DoIt d;
    d = new DoIt();
    d.doit(arg1);
  }
  }
}