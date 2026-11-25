
class Main extends Object {
  void main(String[] args) { 
    int i;
    System.out.printf("%s %n", (i = 4));
    int j;
    j = (i = 42);
    System.out.printf("%s %n", j);
    System.out.printf("%s %s %s %s %n", ((i = 2) + j),i,(i = (5 + j)),i);
    System.out.printf("%s %s %s %s %s %s %s %n", (i = (4 + j)),i,(j = ((i * 4) + 2)),(j = ((i * 4) + 2)),j,((j = (i * 4)) + 4),j);
    int k;
    k = (j = (i = 4));
    System.out.printf("%s %s %s %n", i,j,k);
    k = (((((j = 1) + j) + (i = (j = 2))) + j) + i);
    System.out.printf("%s %n", k);
  }
  
}