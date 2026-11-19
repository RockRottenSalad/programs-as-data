/*
  Exploratory test example.
*/

class DoIt {
  int h; int m;
  int res;
  
  void doit(int arg1) { 
    int i;
    i = 33 * 7 + 8 / 4 - 7 % (2 + 1) + arg1;
    boolean b;
    b = !false && true || i == 2 + 1 && (i != i || i < 2 || i >= 2);
    print(b);
    //String s;
    //s = "jkf\n\t";
    this.h = 23;
    this.m = 59;
    this.m3(6,18);
    // int res; you are not allowed to shadow local variables in java;
    // but you can shadow local variable over field.
    this.res = this.m2();
    {
      int res;			// shadow the class field res in the block.
      res = 12345;
      println(res);
    }
    println(this.res);
    Time t1; 
    t1 = new Time();
    t1.init(12, 35);
    println(t1.getHours());
    println(t1.getMin());
  }      
  
  int m2() {
    boolean bFalse = false;
    if (true)
      while (bFalse)
	if (true)
	  return 111;
    if (true)
      while (bFalse)
	if (true)
	  return 222;
	else
	  return 333;
    if (true)
      while (bFalse)
	if (true)
	  return 444;
	else
	  return 555;
    else
      print(-1 /* "not OK" */ );
    if (true) 
      if (false) 
	return 666;
      else 
	return 777;

    return 999;
  }
  
  void m3(int x, int y) {
    print(this.h); 
    print(this.m);
    int i;
    i = x;
    while (i <= y) {
      print(i);
      i = i + 1;
    }
    return;
  }
}

class Time extends Object {
  int m;
  
  void init(int h, int m) { this.m = 60 * h + m; }

  int getHours() { return this.m / 60; }

  int getMin() { return this.m % 60; }

  void move(int m) { this.m = this.m + m; }
}

class Main {
  void main(int arg1) {
    DoIt d = new DoIt();
    d.doit(arg1);
  }
}
