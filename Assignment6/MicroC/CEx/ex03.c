// micro-C example 3

void main(int n) { 
  int i;         // GETBP; CSTI 1; ADD; i.e. from compile time we know i is local variable 1
  i=0;           // CSTI 0; STI;   because i is at BP + 1
  while (i < n) /* L3 */ {  // BP + 1, get vlaue with LDI, getbp local variable n, if i >= n jmp l2
    /* L2 */
    print i; 
    i=i+1;
  } 
}
