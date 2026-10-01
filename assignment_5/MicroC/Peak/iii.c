void main()
{
    int ns[7];
    ns[0] = 1;
    ns[1] = 2;
    ns[2] = 1;
    ns[3] = 1;
    ns[4] = 1;
    ns[5] = 2;
    ns[6] = 0;
    
    int freq[4];
    freq[0] = 0;
    freq[1] = 0;
    freq[2] = 0;
    freq[3] = 0;    
    
    histogram(7, ns, 3, freq);
    printarr(freq, 4);
}

void histogram(int n, int ns[], int max, int freq[]) {
    int i; 
    i = 0;
    while(i < n) {
        freq[ns[i]] = freq[ns[i]] + 1;
        i = i + 1;
    }
}

void printarr(int arr[], int n)
{
    int i;
    i = 0;
    while (i < n)
    {
        print arr[i];
        i = i + 1;
    }
    println;
}
