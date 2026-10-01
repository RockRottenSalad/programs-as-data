
void main() {
    int x;
    x = 0;
    print ++x; // Should print 1
    print ++x; // Should print 2
    print --x; // Should print 1
    print ++x; // Should print 2
    print --x; // Should print 1
    print --x; // Should print 0
    print --x; // Should print -1
}

