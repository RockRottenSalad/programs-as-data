void histogram(int n, int ns[], int max, int freq[])
{
    int i;
    for(i = 0; i < n; i = i + 1)
    {
        freq[ns[i]] = freq[ns[i]] + 1;
    }
}

void main()
{
    int n;
    n = 8;
    
    int max;
    max = 5;
    
    int ns[8];
    ns[0] = 1;
    ns[1] = 2;
    ns[2] = 0;
    ns[3] = 2;
    ns[4] = 4;
    ns[5] = 2;
    ns[6] = 1;
    ns[7] = 0;
    
    int i;
    
    int freq[5];
    for (i = 0; i < max; i = i + 1)
    {
        freq[i] = 0;
    }
    
    histogram(n, ns, max, freq);
    for (i = 0; i < max; i = i + 1)
    {
        print freq[i];
    }
    println;
}