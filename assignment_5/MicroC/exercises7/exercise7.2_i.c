
void arrsum(int n, int arr[], int *sump)
{
    int i; //declare i
    for (i = 0; i < n; i = i+1)
    {
        *sump = *sump + arr[i];
    }
    print *sump;
}

void main()
{
    int sum; //declare sum
    sum = 0; //instaniate sum as 0
    int *pSum;
    pSum = &sum;
    int arr[4];
    arr[0] = 7;
    arr[1] = 13;
    arr[2] = 9;
    arr[3] = 8;
    arrsum(4, arr, pSum);
    print sum;
}