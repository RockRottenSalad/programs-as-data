
class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    LinkedList lst;
    lst = new LinkedList();
    while ((0 < n)) { 
      lst.addLast(n);
      n = (n - 1);
    }
    lst.printForwards();
    System.out.printf("%n");
    System.out.printf("");
    lst.printBackwards();
  }
  }
}

class Node extends Object {
  Node next;

  Node prev;

  int item;
}

class LinkedList extends Object {
  Node first;

  Node last;

  void addLast(int item) { 
    Node node;
    node = new Node();
    node.item = item;
    if ((this.last == null)) { 
      this.first = node;
      this.last = node;
    } else { 
      this.last.next = node;
      node.prev = this.last;
      this.last = node;
    }
  }

  void printForwards() { 
    Node node;
    node = this.first;
    while ((node != null)) { 
      System.out.printf("%s ", node.item);
      node = node.next;
    }
  }

  void printBackwards() { 
    Node node;
    node = this.last;
    while ((node != null)) { 
      System.out.printf("%s ", node.item);
      node = node.prev;
    }
  }
}