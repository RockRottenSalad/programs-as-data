/*
  Overriding and Dynamic Dispatch
  
  In class hierarchies you may re-define methods with same method
  signatures.

  The method called is based on the runtime type of the object
  containing the methods.

  The method to call is the one closest up the chain with the outset
  in the runtime object type. This is called dynamic dispatch.

   It is impossible to call a method in a superclass that has been
   overrided; unless using super from within class definition, hence
   in a local method. Calling a method using super, e.g., super.m() is
   a direct call resolved at compile time.
 
 */

class A {
  void m() { println(42); }
}

class B extends A {
  void m() { println(43); }
  void superm() { super.m(); }
}

class Main {
  void main() {
    A a = new A();
    B b = new B();
    A ba = b;
    
    a.m();      // 42
    b.m();      // 43
    ba.m();     // 43  
    b.superm(); // 42
  }
}
