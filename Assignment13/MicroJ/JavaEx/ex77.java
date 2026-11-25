// Valid Java variable names are defined in jls25, §3.8
// Start with letter: a-z A-Z _ $
// Continue with letter or digit: a-z A-Z 0-9 _ $
// Constraint on _ : must be a atleast to letters as _ is a keyword.

class Main {
  void main() {
    int a = 1;
    int A = 2;
    int z = 3;
    int Z = 4;
    int __ = 5;
    int _a = 6;
    int $ = 7;
    int $a = 8;

    println(a, A, z, Z, __, _a, $, $a);  // 1 2 3 4 5 6 7 8
  }
}
