// Valid Java variable names are defined in jls25, §3.8
// Start with letter: a-z A-Z _ $
// Continue with letter or digit: a-z A-Z 0-9 _ $
// Constraint on _ : must be a atleast to letters as _ is a keyword.
// Valid test: ex77.java
// Fails with - in name.

class Main {
  void main() {
    int a-b = 1;
  }
}
