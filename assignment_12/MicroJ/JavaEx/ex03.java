/*
  Hiding
  
  Hiding of class fields follows the static, compile time type of the
  variable at the usage.

  From a compile time environment point of view, we need all copies of
  fields with same name as they are all potentially used through
  methods usage or direct object access and type cast. This means that
  field environment must keep track of which fields exists in what
  classes to get the right offset. The compile time type of object
  must be known to access field belonging to that usage.

  This is implemented at runtime using an object layout in memory
  containing a fixed header followed by cells used for all class
  fields, ordered from top and down the chain. The compile time
  environment must map each class field to the offset into the object
  layout.

  Looking up in the compile time environment requires knowledge of the
  hierarchy chain such that correct instance can be found.

  For instance, lookup FldEnv(compile time type F, field "i") returns
  2, because compile time type D is the first seen going up chain and
  D_i has offset 2.

  A compile time class environment is used to map class names to field
  environments:
   ClassEnv: ClassName -> FldEnv.

  Exmaple: ["A" -> FldEnvA, "B" -> FldEnvB, "C" -> FldEnvC (= FldEnvB), ... ]

  A field environment maps all possible class fields to their offset
  in the object layout. The fields are laid out from root class and
  down the hierarchy. The field environment is shown next to each
  class definition below.

*/

class A { int i; }              // FldEnvA: [A_i -> 0]
class B extends A { int i; }    // FldEnvB: [A_i -> 0, B_i -> 1]
class C extends B { }           // FldEnvC = FldEnvB
class D extends C { int i; }    // FldEnvD: [A_i -> 0, B_i -> 1, D_i -> 2]
class E extends D { }           // FldEnvE = FldEnvD
class F extends E { }           // FldEnvF = FldEnvE
class G extends F { int i; }    // FldEnvG: [A_i -> 0, B_i -> 1, D_i -> 2, G_i -> 3]
class H extends G { }           // FldEnvH = FldEnvG
class I extends C { int i; }    // FldEnvI: [A_i -> 0, B_i -> 1, I_i -> 2]


class Main {
  void main() {

    // Testing finding first field up the chain
    A a = new A(); a.i = 1;
    B b = new B(); b.i = 2;
    C c = new C(); c.i = 3;
    D d = new D(); d.i = 4;
    E e = new E(); e.i = 5;
    F f = new F(); f.i = 6;
    G g = new G(); g.i = 7;
    H h = new H(); h.i = 8;
    I i = new I(); i.i = 9;

    println (a.i, b.i, c.i, d.i, e.i, f.i, g.i, h.i, i.i ); // 1 2 3 4 5 6 7 8 9
    
    // Testing finding correct compile time type field based on casting.
    a = h; a.i = 1;
    b = h; b.i = 2;
    c = h;
    d = h; d.i = 3;
    e = h;
    f = h;
    g = h; g.i = 4;

    println (a.i, b.i, c.i, d.i, e.i, f.i, g.i, h.i); // 1 2 2 3 3 3 4 4
  }
}

