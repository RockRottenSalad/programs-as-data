void main() {
    int x;
    x = 0;
    true ? x = x + 1 : x = x - 1;
    print x;
    (false ? x = x - 1 : (true ? x = x + 1 : x = x - 1));
    print x;
    println;
}