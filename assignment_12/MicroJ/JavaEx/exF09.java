
// Non distinct class names.
// Rule: CT-Prog

class A { }
class B extends A { }
class A extends B { }
class Object extends A {

}
