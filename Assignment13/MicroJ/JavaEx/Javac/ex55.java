
class Data extends Object {
  void set(int d) { 

  }

  void set(boolean d) { 

  }

  boolean getB() { 
    return false;
  }

  int getI() { 
    return 0;
  }

  void print() { 

  }
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
    System.out.printf("%s ", this.data);
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
    System.out.printf("%s ", this.data);
  }
}

class Elem extends Object {
  Data data;

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

  void addLast(Data data) { 
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

  void init(int n,Data d) { 
    while ((n > 0)) { 
      this.addLast(d);
      n = (n - 1);
    }
  }

  void upd(int idx,Data data) { 
    Elem elem;
    elem = this.findNth(idx);
    if ((elem != null)) elem.data = data; else { 

    }
  }

  Data get(int idx) { 
    Elem elem;
    elem = this.findNth(idx);
    if ((elem != null)) return elem.data; else return null;
  }

  void printarr() { 
    if ((this.first == null)) return; else { 
      Elem cur;
      cur = this.first;
      while ((cur != null)) { 
        cur.data.print();
        cur = cur.next;
      }
      System.out.printf("%n");
    }
  }
}

class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    int numSol;
    numSol = 0;
    int i;
    int u;
    LinkedList used;
    used = new LinkedList();
    used.init(100,new BData());
    LinkedList diag1;
    diag1 = new LinkedList();
    diag1.init(100,new BData());
    LinkedList diag2;
    diag2 = new LinkedList();
    diag2.init(100,new BData());
    LinkedList col;
    col = new LinkedList();
    col.init(100,new IData());
    BData false_;
    false_ = new BData();
    false_.set(false);
    BData true_;
    true_ = new BData();
    true_.set(true);
    u = 1;
    while ((u <= n)) { 
      used.upd(u,false_);
      u = (u + 1);
    }
    u = 1;
    while ((u <= (2 * n))) { 
      diag1.upd(u,false_);
      diag2.upd(u,false_);
      u = (u + 1);
    }
    i = 1;
    u = 1;
    while ((i > 0)) { 
      while ((i <= n)&&(i != 0)) { 
        while ((u <= n)&&used.get(u).getB()||diag1.get(((u - i) + n)).getB()||diag2.get((u + i)).getB()) u = (u + 1);
        if ((u <= n)) { 
          IData d;
          d = new IData();
          d.set(u);
          col.upd(i,d);
          used.upd(u,true_);
          diag1.upd(((u - i) + n),true_);
          diag2.upd((u + i),true_);
          i = (i + 1);
          u = 1;
        } else { 
          i = (i - 1);
          if ((i > 0)) { 
            u = col.get(i).getI();
            used.upd(u,false_);
            diag1.upd(((u - i) + n),false_);
            diag2.upd((u + i),false_);
            u = (u + 1);
          } else { 

          }
        }
      }
      if ((i > n)) { 
        numSol = (numSol + 1);
        int j;
        j = 1;
        while ((j <= n)) { 
          System.out.printf("%s ", col.get(j).getI());
          j = (j + 1);
        }
        System.out.printf("%n");
        i = (i - 1);
        if ((i > 0)) { 
          u = col.get(i).getI();
          used.upd(u,false_);
          diag1.upd(((u - i) + n),false_);
          diag2.upd((u + i),false_);
          u = (u + 1);
        } else { 

        }
      } else { 

      }
    }
    System.out.printf("%n");
    System.out.printf("%s %n", numSol);
  }
  }
}