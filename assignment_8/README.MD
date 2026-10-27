# 10.1
>To understand how the abstract machine and the garbage collector work and how they collaborate, answer these questions:

>(i) Write 3-10 line descriptions of how the abstract machine executes each of the following instructions:

* `ADD`: Untags the two top most values in the stack, and adds them together.
Tags the result and makes `stack[sp-1]` equal to the result.
decreases the sp making the result the new top most value in the stack.

* `CSTI`: tags `p[pc++]` to int such that it can be Distingushed from an addresse. 
Pushes it on top of stack top.
Increases the stack pointer by 1.

* `NIL`: Pushes `NILVALUE` on top of stack top.
Increases the stack pointer by 1.
The difference between `NILVALUE` and `CSTI 0` is that `NILVALUE` is not tagged.

* `IFZERO`: Untag stack value under this instruction.
If it's a int untag it, and if the int equals 0 jump to `p[pc]`.
Otherwise increase `pc` by 1. 

* `CONS`: allocates space in heap for `p`, a cons cell.
Make two stack top values the `p[1]` and `p[2]`, which is first and second value in cons cell.
Replace the second stack top values with a pointer to `p`, which is a pointer to the cons cell.
Decrease the stack pointer making the pointer to `p` the New stack top value.

* `CAR`: Casts stack pointer as word pointer `p`.
Makes null check on p.
evaluates `p[1]`, first value of cons cell `p`, and pushes it to stacktop.

* `SETCAR`: Casts stacktop as word `v`.
Casts second most stack top value as word pointer `p`.
Updates second word in cons cell `p`, `p[2]`, to `p[2] = v`.

>(ii) Describe the result of applying each C macro Length, Color and Paint from Sect. 10.7.4 to a block header ttttttttnnnnnnnnnnnnnnnnnnnnnngg, that is, a 32-bit word, as described in the source code comments.

the 64-bit block header `ttttttttnnnnnnnnnnnnnnnnnnnnnngg` has 8 tag bits, 2 garbage collection bits and the rest are length bits. The length bits indicate the number of words in the block in additition to the header word. 2 garbage collection bits are for handling freeing blocks. 


>(iii) When does the abstract machine, or more precisely, its instruction interpretation loop, call the allocate(...) function? Is there any other interaction between the abstract machine (also called the mutator) and the garbage collector?

when `CONS` or `HEAPALLOC`

>(iv) In what situation will the garbage collector’s collect(...) function be called?

when there is no more free space, the mutator calls the garbage collector, then attempts to allocate space again.

# 10.2
>Add a simple mark-sweep garbage collector to listmachine.c

I will be using microvm.c instead. In `markPhase` we'll iterate through the word array `s[]`, for each word check if all the following are true for `s[i]`:
* Is not an int.
* Is not `NILVALUE`.
* Is in heap.

Then mark `s[i]`.
```C
void markPhase(word s[], word sp) {
  for(int i = 0; i<=sp; i++){
    if(!IsInt(s[i]) && s[i]!= NILVALUE && inHeap((word *)s[i])){
      mark(((word *) s[i]));
    }
  }
  if (!silent) printf("GC[");
  if (!silent) printf("M");
}
```
Where mark is a function that paints a block black. Incase of cylic structures, if it's already black simply return. 
Otherwise we check that `block[i]` isn't `NILVALUE` and is in heap. 
For the length in header, if `block[i]` is refering to an addresse, by checking if it's not an int, then paint it black.
This concludes the mark phase.
```C
void mark(word *block){ //CHANGED
  if (Color(*block) == Black) return;
  *block = Paint(*block, Black);
  int max = Length(*block);
  for(int i = 1; i<=max;i++){
    if (!IsInt(block[i]) && block[i] != NILVALUE && inHeap((word *)block[i])){
      mark((word *)block[i]);
    }
  }
}
```

For the sweep phase we go through each header in heap.
If color is black, we now paint it as white for next phase, and continue to next iteration.
If color is white, we change the color to blue and add it to the freelist.

```C
void sweepPhase() { // CHANGED
  word *p = (word *)heap;
  while(inHeap(p)){
    int size = Length(p[0]);

    if(Color(p[0])==Black){
      p[0] = Paint(p[0], White);
      p += size +1;
      continue;
    }

    if (Color(p[0])==White){
      p[0] = Paint(p[0], Blue);
      p[1] = (word)freelist;
      freelist = p;
    }
    p += size +1;
  }
  if (!silent) printf(",");
  if (!silent) printf("BS]");
}
```
To run the program first compile microvm.c:
```shell
gcc -Wall microvm.c -o microvm
```
Then run with:
```shell
./microvm ex55.out 10
```
# 10.3
> Improve the sweep phase so that it joins adjacent dead blocks into a single dead block. More precisely, when sweep finds a white (dead) block of length n at address p, it checks whether there is also a white block at address p + 1 + n, and if so joins them into one block.

If the color is white we now use a while loop.
The loop keep tracks of total amount of blocks needed to be merged, `amount`.
When the next block is not white, use `mkheader(...)` function to update old header, where the length is `amount`.
```c
void sweepPhase() { // CHANGED
  word *p = (word *)heap;
  while(inHeap(p)){
    int size = Length(p[0]);

    if(Color(p[0])==Black){
      p[0] = Paint(p[0], White);
      p += size +1;
      continue;
    }
    
    if (Color(p[0]) == White) {
      word *tmp = p;
      int amount = 0;
      while (Color(tmp[0]) == White && inHeap(tmp)) {
        int s = Length(tmp[0]);
        amount += s + 1;
        tmp += s + 1;
      }
      p[0] = mkheader(0, amount-1, Blue);
      p[1] = (word)freelist;
      freelist = p;
      p = tmp; 
      continue;
      }
    p += size + 1;
  }
  if (!silent) printf(",");
  if (!silent) printf("BS]");
}
```