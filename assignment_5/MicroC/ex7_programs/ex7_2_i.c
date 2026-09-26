
void main() { 
    int sum;
    int arr[4];
    arr[0] = 7;
    arr[1] = 13;
    arr[2] = 9;
    arr[3] = 8;
    arrsum(4, arr, &sum);
    print sum;
}

void arrsum(int n, int arr[], int *sump) {
    int sum;
    int i;
    sum = 0;
    i = 0;
    while(i < n) {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
