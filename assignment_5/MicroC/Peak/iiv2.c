void main(int n)
{
    int a[10];
    int sum;
    squares2(n, a);
    arrsum2(n, a, &sum);
    
    print sum;
    println;
}

void squares2(int n, int arr[])
{
    int i;
    for (i = 0; i < n; ++i)
    {
        arr[i] = i * i;
    }
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