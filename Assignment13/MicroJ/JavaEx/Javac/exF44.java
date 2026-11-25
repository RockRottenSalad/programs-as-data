
class Main extends Object {
  int f(boolean b) { 
    if (b) { 
      System.out.printf("%s ", 1);
      return 1;
    } else { 
      System.out.printf("%s ", 2);
    }
  }

  void main(String[] args) { 
    this.f(true);
  }
  
}