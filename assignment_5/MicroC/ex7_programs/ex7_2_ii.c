
void main(int n) { 
    int sumofsquares;
    int arr[20];
    squares(n, arr);
    arrsum(n, arr, &sumofsquares);
    print sumofsquares;
}

void squares(int n, int arr[]) {
    int i; i = 0;
    while(i < n) {
        arr[i] = i * i;
        i = i + 1;
    }
}


void arrsum(int n, int arr[], int *sump) {
    int i;
    int sum;
    i = 0;
    sum = 0;
    while(i < n) {
        sum = sum + arr[i];
        i = i + 1;
    }
    *sump = sum;
}
