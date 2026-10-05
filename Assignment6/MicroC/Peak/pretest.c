void main() 
{
    int i;
    i = 0;
    int a[4];
    a[0] = 1;
    a[1] = 2;
    a[2] = 3;
    a[3] = 4;
    
    // ++i has a side effect, and we want it to execute only once, i.e. its not ok to do
    // arr[++i] = arr[++i] + 1
    // after the expression we want i to be 1, and arr[1] to be 3
    print ++a[++i]; // we would expect this to print  3
    print i;          // should print 1
    
    // here i should be 0 and arr[0] should be 0
    print --a[--i]; // we would expect this to print  0
    print i;          // should print 0
}