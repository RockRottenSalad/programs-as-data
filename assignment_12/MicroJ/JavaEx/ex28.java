// Shows Dangling else problem, jls25, Sec.14.5

class Main {
  void ifs(boolean b1, boolean b2) {
    if (b1)
      if (b2)
        print(1);
    else
      print(2);

    println(3);
  }
  
  void main() {
    this.ifs(false,false);  // 3
    this.ifs(true,false);   // 2 3 - the else belongs to inner-most if.
    this.ifs(false,true);   // 3 - there is no else to b1.
    this.ifs(true,true);    // 1 3 
  }
}

