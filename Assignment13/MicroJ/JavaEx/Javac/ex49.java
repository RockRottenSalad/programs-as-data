
class A extends Object {
  boolean isPrime(int n) { 
    int k;
    k = 2;
    while (((k * k) <= n)&&((n % k) != 0)) k = (k + 1);
    return (n >= 2)&&((k * k) > n);
  }
}

class Main extends Object {
  void main(String[] args) {
    int range = Integer.parseInt(args[0]); { 
    A a;
    a = new A();
    int count;
    int i;
    count = 0;
    i = 2;
    while ((i < range)) { 
      if (a.isPrime(i)) count = (count + 1); else { 

      }
      i = (i + 1);
    }
    System.out.printf("%s ", range);
    System.out.printf("%s ", count);
    System.out.printf("%n");
  }
  }
}