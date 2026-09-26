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
    
    histogram2(7, ns, 3, freq);
    
    printarr(freq, 4);
}

void histogram(int n, int ns[], int max, int freq[])
{
    int count;
    
    int c;
    c = 0;
    while (c < max + 1)
    {
        count = 0;
        int i;
        i = 0;
        
        while (i < n)
        {
            if (ns[i] == c)
            {
                count = count  +1;
            }
            i = i + 1;
        }
        
        freq[c] = count;
        c = c + 1;
    }
}

void histogram2(int n, int ns[], int max, int freq[])
{
    int count;
    
    
    int c;
    for (c = 0; c <= max; ++c)
    {
        count = 0;
        int i;
        for (i = 0; i < n; ++i)
        {
            if (ns[i] == c)
            {
                count += 1;
            }            
        }
        freq[c] = count;
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
