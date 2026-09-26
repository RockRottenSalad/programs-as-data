void main()
{
    int a[10];
    int sum;
    squares(10, a);
    arrsum(10, a, &sum);
    
    print sum;
    println;
}

void squares(int n, int arr[])
{
    int i;
    i = 0;
    while (i < n)
    {
        arr[i] = i * i;
        i = i + 1;
    }
}

void squares2(int n, int arr[])
{
    int i;
    for (i = 0; i < n; ++i)
    {
        arr[i] = i * i;
    }
}

// arrsum from the previous exercise
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
    for(i = 0; i < n; i = i + 1)
    {
        sum += arr[i];
    }
    *sump = sum;
}