
class Main extends Object {
  void ifs(boolean b1,boolean b2) { 
    if (b1) if (b2) System.out.printf("%s ", 1); else System.out.printf("%s ", 2); else { 

    }
    System.out.printf("%s %n", 3);
  }

  void main(String[] args) { 
    this.ifs(false,false);
    this.ifs(true,false);
    this.ifs(false,true);
    this.ifs(true,true);
  }
  
}