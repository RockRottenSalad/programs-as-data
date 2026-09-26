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


void arrsum(int n, int  arr[], int *sump)
{
    int sum;
    int i;
    i = 0;
    sum = 0;
    
    while (i < n)
    {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
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