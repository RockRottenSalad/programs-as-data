void main(int n)
{
    int a[20];
    int sum;
    squares(n, a);
    arrsum(n, a, &sum);
    
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