// micro-Java - compute and print array of factorials
// Array is implemented as a linked list.

class Elem {
  int data;
  Elem next;
}

class LinkedList {
  Elem first;

  Elem findLast() {
    if (this.first == null)
      return null;
    else {
      Elem cur = this.first;
      while (cur.next != null)
        cur = cur.next;
      return cur;
    }
  }

  Elem findNth(int n) {
    if (this.first == null)
      return null;
    else {
      Elem cur = this.first;
      while (n > 0 && cur.next != null) {
        cur = cur.next;
        n = n - 1;
      }
      if (n == 0)
        return cur;   // found n'th element.
      else
        return null;  // n'th element not in list.
    }
  }
  
  void addLast(int data) {
    Elem elem = new Elem();
    elem.data = data;
    elem.next = null;
    if (this.first == null)
      this.first = elem;
    else {
      Elem last = this.findLast();
      last.next = elem;
    }
  }

  void init(int n) {
    while (n > 0) {
      this.addLast(0);
      n = n - 1;
    }
  }

  void upd(int idx, int data) {
    Elem elem = this.findNth(idx);
    if (elem != null)
      elem.data = data;
  }

  int get(int idx) {
    Elem elem = this.findNth(idx);
    if (elem != null)
      return elem.data;
    else
      return 0;
  }

  void printarr() {
    if (this.first == null)
      return;
    else {
      Elem cur = this.first;
      while (cur != null) {
        print(cur.data);
        cur = cur.next;
      }
      println();
    }
  }
}    

class Main {
  void main(int n) {
    LinkedList a;    
    a = new LinkedList();
    a.init(n);
    
    int i = 0; 
    int f = 1;
    while (i < n) {
      a.upd(i,f);
      i = i + 1;
      f = f * i;
    }
    a.printarr();
  }
}
