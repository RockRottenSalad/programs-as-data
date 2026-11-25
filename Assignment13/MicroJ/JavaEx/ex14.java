/*
   Field hiding with same name but different type.

*/

class B extends Object { 
  int x; 
  
  int m() { 
    return this.x * 7;
  } 
}

class C extends B { 
  boolean x; 
}

class Main extends Object {
  void main() {
    C c;
    c = new C();
    c.x = false;
    // The field B_x is not initialized and result of c.m() is 0.    
    println(c.x, c.m()); // false 0
  }
}
