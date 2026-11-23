/* 
   A Date class

   Strings are excluded for now.
*/

class Main extends Object {
  void main(int y, int m, int d) { 
    Date d1;
    d1 = new Date();
    d1.init(2001, 2, 14);
    print(d1.weekday());
    print(d1.dayno());
    //print(d1.toString());
    Date d2;
    d2  = new Date();
    d2.y = y;
    d2.m = m;
    d2.d = d;
    print(d1.to(d2));
    print(d2.weekday());
    //print(d2.toString());
  }      
}

class Date extends Object {
  int y; int m; int d;
  
  void init(int y, int m, int d) {
    this.y = y;  this.m = m; this.d = d;
  }

  boolean isLeap() {
    return this.y % 4 == 0 && this.y % 100 != 0 || this.y % 400 == 0;
  }

  int dayinyear() {
    int m3;
    m3 = (this.m+9)%12;
    int res;
    res = (m3/5*153 + m3%5*30 + (m3%5+1)/2 + 59) % 365 + this.d;
    if (this.isLeap())
      return res + 1;
    else
      return res;
  }

  // Number of days since the (hypothetical) Monday 0001-01-01
  int dayno() {
    int y1;
    y1 = this.y-1;
    return this.dayinyear() + 365*y1 + y1/4 - y1/100 + y1/400 - 1;
  }

  // 0 = Monday, 1 = Tuesday, ..., 6 = Sunday
  int weekday() {
    return this.dayno() % 7;
  }

  // Difference between two dates
  int to(Date t2) {
    return t2.dayno() - this.dayno();
  }

  /*
  String toString() { 
    return tostring(this.y) + "-" 
         + tostring(this.m) + "-" 
         + tostring(this.d);
         }*/
}
