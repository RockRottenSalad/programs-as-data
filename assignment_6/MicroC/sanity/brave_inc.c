
void main() {
    int i;
    int arr[3];
    arr[0] = 10;
    arr[1] = 20;
    arr[2] = 30;
    i = 0;

    ++arr[++i];

    // expected array contents: 10 21 30 
    // expected i: 1
    print arr[0];
    print arr[1];
    print arr[2];
    println;
    print i;
}
