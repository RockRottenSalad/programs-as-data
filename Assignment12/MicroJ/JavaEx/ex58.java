// Testing assign as an expression

class Main {
  void main() {
    int i;
    println(i=4);  // 4

    int j = i = 42;
    println (j);   // 42

    println ((i=2)+j, i, i=5+j, i); // 44 2 47 47

    println (i=4+j, i, j=i*4+2, j=(i*4)+2, j, (j=i*4)+4,j); // 46 46 186 186 186 188 184

    int k = (j=i=4);
    println(i,j,k); // 4 4 4

    k = (j=1) + j + (i=j=2) + j + i;
    println(k); // 8    
  }
}
