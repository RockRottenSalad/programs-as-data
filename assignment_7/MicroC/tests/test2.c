void main()
{
    int month;
    int y;
    int days;
    
    month = 2;
    y = 2020;
    
    switch (month) {
    case 1:
        { days = 31; }
    case 2:
        { days = 28; if (y%4==0) days = 29; }
    case 3:
        { days = 31; }
    }
    print days;
    println;
}