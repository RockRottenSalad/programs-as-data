
class Main extends Object {
  int count;

  void main(String[] args) { 
    this.count = 0;
    System.out.printf("%s ", this.tak(10,5,0));
    System.out.printf("%s ", this.count);
  }
  

  int tak(int x,int y,int z) { 
    this.count = (this.count + 1);
    if ((y < x)) return this.tak(this.tak((x - 1),y,z),this.tak((y - 1),z,x),this.tak((z - 1),x,y)); else return z;
  }
}