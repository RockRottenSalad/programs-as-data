// A C++ example illustrating multiple inheritance with a diamond
// structure. 
//
//      A
//     / \
//    B   C
//     \ /
//      D
//
// C++ does not allow chaining up the hierarchy, e.g., you can't
// directly access field A.i via e.g., D.B.A.i or D.C.A.i.
//
//   D
//   ├── B
//   │   └── A
//   └── C
//       └── A
//
// You can go up one layer why the example below declares functions
// setI and getI in class A.
//
// In this example the base class A is repeated in class B and C,
// i.e., instance objects of class D has two different instance
// objects of type A through the objects for classes B and C. This is
// achieved by NOT using keyword virtual.
//
// Reference: https://cppreference.com/cpp/language/derived_class

#include <iostream>

class A {
public:
  int i = 0;
  void setI(int i) {
    this->i = i;
  }
  int getI() {
    return this->i;
  }
};
  
class B : public A {
public:
  int i = 1;
};

class C : public A {
public:
  int i = 2;
};

class D : public B, public C {
public:
  int i = 3;
};

int main() {
  D d;  // Instance object d of type D created.

  std::cout << d.i << '\n';         // 3 - field D.i
  std::cout << d.B::i << '\n';      // 1 - field B.i
  std::cout << d.C::i << '\n';      // 2 - field C.i
  std::cout << d.B::getI() << '\n'; // 0 - field A.i in class B
  std::cout << d.C::getI() << '\n'; // 0 - field A.i in class C

  d.B::setI(42);                    // field A.i in class B
  std::cout << d.B::getI() << '\n'; // 42 - field A.i in class B
  std::cout << d.C::getI() << '\n'; // 0 - field A.i in class C - two different instances of A in B and C.  
}
