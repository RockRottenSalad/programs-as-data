// micro-Java - n queens problem 

// Array is implemented as a linked list.

// Number of solutions:
// Size n    Solutions
//      1          1
//      2          0
//      3          0
//      4          2
//      5         10
//      6          4
//      7         40
//      8         92
//      9        352
//     10        724
//     11       2680
//     12      14200
//     13      73712
//     14     365596
//     15    2279184


class Data {

  void set(int d) { }
  void set(boolean d) { }
  boolean getB() { return false; }
  int getI() { return 0; }
  void print() { }
}

class BData extends Data {
  boolean data;

  void set(boolean d) {
    this.data = d;
  }

  boolean getB() {
    return this.data;
  }

  void print() {
    print(this.data);
  }
}

class IData extends Data {
  int data;

  void set(int d) {
    this.data = d;
  }

  int getI() {
    return this.data;
  }

  void print() {
    print(this.data);
  }
}

class Elem {
  Data data;
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
  
  void addLast(Data data) {
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

  void init(int n, Data d) {
    while (n > 0) {
      this.addLast(d);
      n = n - 1;
    }
  }

  void upd(int idx, Data data) {
    Elem elem = this.findNth(idx);
    if (elem != null)
      elem.data = data;
  }

  Data get(int idx) {
    Elem elem = this.findNth(idx);
    if (elem != null)
      return elem.data;
    else
      return null;
  }

  void printarr() {
    if (this.first == null)
      return;
    else {
      Elem cur = this.first;
      while (cur != null) {
        cur.data.print();
        cur = cur.next;
      }
      println();
    }
  }
}    

class Main {
  
  void main(int n) {
    int numSol = 0;
    int i; 
    int u;
    LinkedList used = new LinkedList();
    used.init(100, new BData());
    LinkedList diag1 = new LinkedList();
    diag1.init(100, new BData());
    LinkedList diag2 = new LinkedList();
    diag2.init(100, new BData());
    LinkedList col = new LinkedList();
    col.init(100, new IData());

    BData false_ = new BData();
    false_.set(false);
    BData true_ = new BData();
    true_.set(true);

    u = 1;
    while (u <= n) {
      used.upd(u,false_);
      u = u+1;
    }

    u = 1;
    while (u <= 2 * n) {
      diag1.upd(u,false_);
      diag2.upd(u,false_);
      u = u+1;
    }

    i = 1;
    u = 1;
    while (i > 0) {
      while (i <= n && i != 0) {
        while (u <= n && (used.get(u).getB() || diag1.get(u-i+n).getB() || diag2.get(u+i).getB()))
          u = u + 1;
        if (u <= n) { // not used[u]; fill col[i] then try col[i+1]
          IData d = new IData();
          d.set(u);
          col.upd(i,d); 
          used.upd(u,true_);
          diag1.upd(u-i+n,true_);
          diag2.upd(u+i,true_);
          i = i+1; u = 1;
        } else {			// backtrack; try to find a new col[i-1]
          i = i-1;
          if (i > 0) { 
            u = col.get(i).getI(); 
            used.upd(u,false_);
            diag1.upd(u-i+n,false_);
            diag2.upd(u+i,false_);
            u = u+1;
          } 
        }
      }

      if (i > n) {                // output solution, then backtrack
        numSol = numSol + 1;
        int j;
        j = 1;
        while (j <= n) {
          print (col.get(j).getI());  
          j = j+1;
        }
        println();
        i = i-1; 
        if (i > 0) { 
          u = col.get(i).getI(); 
          used.upd(u,false_);
          diag1.upd(u-i+n,false_);
          diag2.upd(u+i,false_);
          u = u+1;
        }
      }
    }
    println();
    println(numSol);
  }
}
