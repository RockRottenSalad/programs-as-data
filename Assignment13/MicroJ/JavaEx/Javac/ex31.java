
class Main extends Object {
  int f(boolean b) { 
    if (b) { 
      System.out.printf("%s %n", 1);
      return 1;
    } else { 
      System.out.printf("%s %n", 2);
      return 2;
    }
  }

  void main(String[] args) { 
    this.f(true);
    this.f(true);
  }
  
}