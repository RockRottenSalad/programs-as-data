/*
  Overriding, Dynamic Dispatch, vTable

  Overriding of methods follows the runtime type of the object at the
  usage.

  At any point in time, only one method, m, can be called from an
  object o, o.m. The method is found based on the runtime type of o
  and finding the closest m going up the chain.

  super.m is resolved at compile time to a direct call and hence not
  part of dynamic dispatch.
  
  The vTable is a compile time generated table containing addresses of
  methods to call based on runtime type of object. That is, each class
  has a vTable containing one version of each method available. 
  
  A vTable is unique for each class, but as illustrated below, can be
  shared when they refer to same set of methods.

  vTables are laid out at compile time and part of byte code.

  A compile time class environment, ClassEnv, will map class names to
  vTables, e.g.: ClassEnv: ClassName -> vTable.

  For instance
    ["A" -> vTableA, "B" -> vTableB, "C" -> vTableC (= vTableB), ... ]

  The ClassEnv is used to get vTable information when generating the
  vTable byte code.
  
 */

class A { int f() { return 1; } }            // Class A vTable: [f_A]
class B extends A { int f() { return 2; } }  // Class B vTable: [f_B] - only way to call f_A is using super.
class C extends B { }                        // Class C vTable: [f_B]
class D extends C { int f() { return 3; } }  // Class D vTable: [f_D]
class E extends D { }                        // Class E vTable: [f_D]
class F extends E { }                        // Class F vTable: [f_D]
class G extends F { int f() { return 4; } }  // Class G vTable: [f_G]
class H extends G { }                        // Class H vTable: [f_G]

class Main {

  void main() {
    A a = new A();
    B b = new B();
    C c = new C();
    D d = new D();
    E e = new E();
    F f = new F();
    G g = new G();
    H h = new H();

    println(a.f(), b.f(), c.f(), d.f(), e.f(), f.f(), g.f(), h.f()); // 1 2 2 3 3 3 4 4

    A ab = b;
    A ac = c;
    B bc = c;
    println(ab.f(), ac.f(), bc.f()); // 2 2 2

    A ad = d;
    B bd = d;
    C cd = d;
    println(ad.f(), bd.f(), cd.f()); // 3 3 3

    A ah = h;
    B bh = h;
    C ch = h;
    D dh = h;
    E eh = h;
    F fh = h;
    G gh = h;
    println(ah.f(), bh.f(), ch.f(), dh.f(), eh.f(), fh.f(), gh.f()); // 4 4 4 4 4 4 4
  }
}
