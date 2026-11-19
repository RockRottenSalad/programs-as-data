// micro-Java
// Counting prime numbers 

class A {
  boolean isPrime(int n) {
    int k;
    k = 2;
    while (k * k <= n && n % k != 0)
      k = k + 1;
    return n >= 2 && k * k > n;
  }
}

class Main {
  void main(int range) {
    A a = new A();
    int count;
    int i;
    count = 0;
    i = 2;
    while (i < range) {
      if (a.isPrime(i))
        count = count + 1;
      i = i + 1;
    }
    print(range);
    print(count);
    println();
  }
}
