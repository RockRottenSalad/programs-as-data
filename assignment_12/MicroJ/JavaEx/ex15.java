/* 
   Doubly-linked lists in microJ
*/


class Main extends Object {
  void main(int n) {
    LinkedList lst;
    lst = new LinkedList();
    while (0 < n) {
      lst.addLast(n);
      n = n - 1;
    }
    lst.printForwards();
    println();
    print();
    lst.printBackwards();
  }
}

class Node extends Object {
  Node next;
  Node prev;
  int item;
}

class LinkedList extends Object {
  Node first;
  Node last;		// Invariant: first==null iff last==null

  void addLast(int item) {
    Node node;
    node = new Node();
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
