/* 
   Test subclasses and fields

   Strings are excluded for now.
*/

class Main extends Object {
  void main() { 
    A a1;
    B a2;
    a1 = new A();
    a1.a = 111;
    a2 = new B();
    a2.seta(222);
    a2.b = 333;
    print(/*a1.toString(), a2.toString(),*/ a2.a, a2.b);
  }      
}

class B extends A {
  int b;
  
  void init(int b) { this.b = b; }

  void set(int x) { this.seta(x); }

  int geta() { return this.a; }

  int getb() { return this.b; }

  /*  void toString() { return "a = " + tostring(this.a) 
      + "; b = " + tostring(this.b); } */
}

class A extends Object {
  int a;
  
  void seta(int a) { this.a = a; }

  int geta() { return this.a; }

  // void toString() { return "a = " + tostring(this.a); }
}

// class Bad extends Bad { }          // Would cause the interpreter to loop
