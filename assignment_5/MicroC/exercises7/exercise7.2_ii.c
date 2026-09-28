void squares(int n, int arr[])
{
    int i;
    for (i = 0; i < n; i = i + 1)       //loops for each element i, in arr, and sets value to i*i up to n
    {
        arr[i] = i * i;
    }
}

void main(int n) {
    int arr[20];
    int i;
    for (i = 0; i < n; i = i + 1)       //loops to instantiate every item in arr up to n
    {
        arr[i] = 0;
    }
    squares(n, arr);
    
    int sum;      
    sum = 0;
    for (i = 0; i < n; i = i + 1)       //loops to increase sum by the value of each item in arr up to n
    {
        sum = sum + arr[i];
    }
    print sum;
    println;                            //making print look more clean
}