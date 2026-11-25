
class Elem extends Object {
  int data;

  Elem next;
}

class LinkedList extends Object {
  Elem first;

  Elem findLast() { 
    if ((this.first == null)) return null; else { 
      Elem cur;
      cur = this.first;
      while ((cur.next != null)) cur = cur.next;
      return cur;
    }
  }

  Elem findNth(int n) { 
    if ((this.first == null)) return null; else { 
      Elem cur;
      cur = this.first;
      while ((n > 0)&&(cur.next != null)) { 
        cur = cur.next;
        n = (n - 1);
      }
      if ((n == 0)) return cur; else return null;
    }
  }

  void addLast(int data) { 
    Elem elem;
    elem = new Elem();
    elem.data = data;
    elem.next = null;
    if ((this.first == null)) this.first = elem; else { 
      Elem last;
      last = this.findLast();
      last.next = elem;
    }
  }

  void init(int n) { 
    while ((n > 0)) { 
      this.addLast(0);
      n = (n - 1);
    }
  }

  void upd(int idx,int data) { 
    Elem elem;
    elem = this.findNth(idx);
    if ((elem != null)) elem.data = data; else { 

    }
  }

  int get(int idx) { 
    Elem elem;
    elem = this.findNth(idx);
    if ((elem != null)) return elem.data; else return 0;
  }

  void printarr() { 
    if ((this.first == null)) return; else { 
      Elem cur;
      cur = this.first;
      while ((cur != null)) { 
        System.out.printf("%s ", cur.data);
        cur = cur.next;
      }
      System.out.printf("%n");
    }
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    LinkedList a;
    a = new LinkedList();
    a.init(n);
    int i;
    i = 0;
    int f;
    f = 1;
    while ((i < n)) { 
      a.upd(i,f);
      i = (i + 1);
      f = (f * i);
    }
    a.printarr();
  }
  }
}