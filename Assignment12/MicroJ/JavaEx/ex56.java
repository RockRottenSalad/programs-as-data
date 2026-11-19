// micro-Java - doubly-linked lists

class Main {
  void main(int n) {
    LinkedList lst;
    lst = new LinkedList();
    while (n > 0) {
      lst.addLast(n+1);
      lst.addLast(n-1);
      n = n - 1;
    }
    Node node = lst.first;

    lst.printForwards();
    println();
    lst.printBackwards();
  }
}

class Node {
  Node next;
  Node prev;
  int item;
}

class LinkedList {
  Node first;
  Node last;		// Invariant: first==null iff last==null

  void addLast(int item) {
    Node node = new Node();
    node.item = item;
    if (this.last == null) {
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
    while (node != null) {
      print(node.item);
      node = node.next;
    }
  }

  void printBackwards() {
    Node node;
    node = this.last;
    while (node != null) {
      print(node.item);
      node = node.prev;
    }
  }
}


