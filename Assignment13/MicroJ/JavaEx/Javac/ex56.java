
class Main extends Object {
  void main(String[] args) {
    int n = Integer.parseInt(args[0]); { 
    LinkedList lst;
    lst = new LinkedList();
    while ((n > 0)) { 
      lst.addLast((n + 1));
      lst.addLast((n - 1));
      n = (n - 1);
    }
    Node node;
    node = lst.first;
    lst.printForwards();
    System.out.printf("%n");
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