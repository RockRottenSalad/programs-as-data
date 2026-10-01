// micro-C example 1

void main()
{
    int a[4];
    a[0] = 7;
    a[1] = 13;
    a[2] = 9;
    a[3] = 8;
    
    int sum;
    arrsum2(4, a, &sum);
    print sum;
    println;
    
}

void arrsum2(int n, int  arr[], int *sump)
{
    int sum;
    sum = 0;
    
    int i;
    for(i = 0; i < n; ++i)
    {
        sum += arr[i];
    }
    *sump = sum;
}