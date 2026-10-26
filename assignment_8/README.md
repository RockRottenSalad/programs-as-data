
# Exercise 10.1

## (i) Write 3-10 line descriptions of how the abstract machine executes each of the following instructions.

> ADD, which adds two integers.

The desired effect of adding two integers in an abstract machine is
to pop off two elements from the stack, add them together and then
push the result onto the stack.

Since the first bit in this machine is used as a tag to indicate if a
value is an int or a pointer, we first untag the value at the stack
pointer and the value before that, i.e `s[sp]` and `s[sp-1]`. Then we
have the actual int values which we simply add together. The result
can then be tagged again to indicate that this is an int, and then we
write at the new stack top `sp-1` and decrement `sp` so that it
points to the new top.

```c
case ADD: 
  s[sp-1] = Tag(Untag(s[sp-1]) + Untag(s[sp])); sp--; break;
```


> CSTI i, which pushes integer constant i.

After the CSTI instruction has been read, the program pointer will be pointing
at the constant we want to push on the stack.

So, we simply read the value at `pc`, then increment `pc` so that it points to
the next instruction. The value read from `pc` is the constant `i`, this needs
to be tagged to indicate that it's an int. Then we write the value on top of
the stack `sp+1` and update the stack pointer accordingly.

```c
case CSTI:
  s[sp+1] = Tag(p[pc++]); sp++; break;
```

> NIL, which pushes a nil reference. What is the difference between NIL and CSTI 0?

This instruction simply writes the value `0` ontop of the stack and updates the stack pointer accordingly.

The difference between `NIL` and `CSTI 0` is that the first is a pointer and
the latter is an integer. Since we use the first bit to indicate that something
is an integer, `CSTI 0` after being tagged will have the bit representation `0b1`. Whereas
`NIL` will simply be `0b0`.

```c
case NIL:    
  s[sp+1] = NILVALUE; sp++; break;
```

> IFZERO, which tests whether an itneger is zero, or a reference is nil.

The idea here is that after the bytecode for `IFZERO` we have the index of the
byte code instruction we should jump to incase the comparison evalutes to
true.

First we read the value we want to compare with `0` by popping a value from the stack. If this value is an integer, then we need to untag it first since the integer `0` has the bit representation `0b1`.

If the value is `0`, we update our PC to the bytecode index that was supplied directly after the `IFZERO` instruction. Otherwise we just increment our PC.

```c
case IFZERO: { 
  word v = s[sp--];
  pc = (IsInt(v) ? Untag(v) == 0 : v == 0) ? p[pc] : pc+1; 
} break;
```


> CONS

Cons is used to allocate a cell in a linked list. The two words after the
`CONS` instructions correspond to the value in the cons element and the tail of
the list.

The allocate function takes a tag which is included in the memory block header
along with the number of words to allocate.

We then write the value and tail of the list into the allocated memory block.
Finally we push the pointer to the cons cell onto the stack by overriding the
value at `sp-1` and then decrementing `sp` so that it points to the cons
pointer. This has the effect of popping off the value as well as the tail and
then pushing the cons pointer on the stack.

```c
case CONS: {
  word* p = allocate(CONSTAG, 2, s, sp); 
  p[1] = (word)s[sp-1];
  p[2] = (word)s[sp];
  s[sp-1] = (word)p;
  sp--;
} break;
```

> CAR

The `CAR` instruction loads the value stored in a `Cons` cell.

From the previous instruction, we know that the first word(after the block
header) is the value of the cell. So after ensuring the pointer isn't null, we
read the word stored at a one word offset from the cons pointer, and override
the pointer on the stack. This has the effect of popping off the pointer from
the stack and then pushing the cons value onto the stack.

```c
word* p = (word*)s[sp]; 
  if (p == 0) 
      { printf("Cannot take car of null\n"); return -1; }
  s[sp] = (word)(p[1]);
```

> SETCAR

The `SETCAR` instruction overrides the value stored in a `Cons` cell.

This instruction expects that the value that should written to the `Cons` cell is stored on top of the stack. It also assumes that the pointer to the `Cons` cell is stored directly below the value on the stack.

First we pop the value off the stack and write it to variable `v`.

Then we read the pointer to the `Cons` cell from the stack.

Then we recall that the value is stored at a 1 word offset due to the header data being placed at the start of every allocated memory block. So we write with a 1 word offset.

```c
case SETCAR: {
  word v = (word)s[sp--];
  word* p = (word*)s[sp]; 
  p[1] = v;
```

## (ii) Describe the result of applying each C macro Length, Color and Paint from Sect. 10.7.4 to a block header ttttttttnnnnnnnnnnnnnnnnnnnnnngg, that is, a 32-bit word, as described in the source code comments.


The `Length` macro extracts the `nnnnnnnnnnnnnnnnnnnnnn` part of the block header. This value indicates the size(measured in words) of the allocated memory block.

The `Color` macro extracts the `gg` part of the block header. These are the garbage collection flags used to paint a block white, grey, black or blue.

The `Paint` macro allows you to override the `gg` flags to your desired color.

## (iii) When does the abstract machine, or more precisely, its instruction interpretation loop, call the allocate(...) function? Is there any other interaction between the abstract machine(also called the mutator) and the garbage collector?


When executing the `CONS` and `HEAPALLOC` instructions, the allocate function is called.

## (iv) In what situation will the garbage collector's collect(...) function be called?

It is only called when  there's no free memory block large enough to accomodate
the requested number of words in the free list.

```c
word* allocate(unsigned int tag, uword length, word s[], word sp) {
  int attempt = 1;
  do {
    word* free = freelist;
    word** prev = &freelist;
    while (free != 0) {
      // Loop body omitted
      // Search for a free block large enough to accomodate "length" words
    }
    // No free space, do a garbage collection and try again
    if (attempt==1)
      collect(s, sp);
  } while (attempt++ == 1);
  printf("Out of memory\n");
  exit(1);
}
```

# Exercise 10.2
> Add a simple mark-sweep garbage collector

First we define a `block_should_be_marked` function which indicates whether if
a block should be marked or not. It does so by making sure the block is a
non-nil pointer and that it hasn't already been marked before(i.e. the block is
still white). The last check mentioned is to ensure we our mark phase doesn't
get stuck in an infinite loop when dealing with cyclic data structures.


```c
bool block_should_be_marked(word block) {
    return !IsInt(block) && block != NILVALUE && Color(*(word*)block) == White; 
}
```

Afterwards we define a function that marks a block by first coloring it black
and then iterating through the whole block for additional blocks that need to
be recursively marked.

```c
void mark(word *block) {
    word block_length = Length(*block);
    *block = Paint(*block, Black);

    for(word i = 1; i <= block_length; i++) {
        if(block_should_be_marked(block[i])) {
            mark((word*)block[i]);
        }
    }
}
```


Finally the entirety of the mark phase can be implemented by searching through
the stack for blocks that need to be marked.

```c
void markPhase(word s[], word sp) {
  if (!silent) printf("GC[");

  // GC phase 1: mark phase
  // Assume all objects are white
  // Find objects reachable from stack and color them black
  for(word i = 0; i < sp; i++) {
      if (block_should_be_marked(s[i])) {
          mark((word*)s[i]);
      }
  }

  if (!silent) printf("M");
}
```

Now we implement the sweep phase. We do this by iterating through the entire
heap and building a new freelist. Blue blocks that aren't orphans were already
on the freelist, so we simply add those to the new freelist. White blocks are
colored blue and then added to the freelist. Finally black blocks are simply
colored white again so our assumption in the sweep phase about blocks initially
being white holds.

```c
void sweepPhase() {
  if (!silent) printf(",");

  // GC phase 2: sweep phase
  // Scan entire heap, put all white objects on the freelist, and color black objects white
  word *block = heap;
  word *new_freelist = NULL;
    
  while (block < afterHeap) {
      word block_length = Length(*block);
      word color = Color(*block);

      switch (color) {
          case Black:
              *block = Paint(*block, White);
            break;
          case White:
              *block = Paint(*block, Blue);
          case Blue:
              // make sure block isn't orphan
              if (block_length > 0) {
                  block[1] = (word)new_freelist;
                  new_freelist = block;
              }
            break;
          default:
      }
      // +1 to account for the fact that the header isn't included in the block  length
      block += block_length + 1;
  }

  freelist = new_freelist;
  if (!silent) printf("BS]");
}
```

# Exercise 10.3

> Improve the sweep phase so that it joins adjacent dead blocks into a single dead block.

This is rather simple to implement. Since we always insert into the front of the new freelist, the head of the new freelist will be equal to the previous block we added.

We simply check that the length of the block at the head of the list plus 1 to account for the header yields the block we're currently inspecting. If it does, then we know that the blocks are adjacent and we merge them by simply updating the length in the header of the block in the freelist.

If they aren't adjacent, then we just do what we did earlier and append the block to the front of the list.

```c
void sweepPhase() {
  if (!silent) printf(",");

  // GC phase 2: sweep phase
  // Scan entire heap, put all white objects on the freelist, and color black objects white
  word *block = heap;
  word *new_freelist = NULL;
  word freelist_head_block_length = 0;
    
  while (block < afterHeap) {
      word block_length = Length(*block);
      word color = Color(*block);

      switch (color) {
          case Black:
              *block = Paint(*block, White);
            break;
          case White:
              *block = Paint(*block, Blue);
          case Blue:
              if(new_freelist != NULL && new_freelist + Length(*new_freelist) + 1 == block) {
                  *new_freelist = mkheader(0, Length(*new_freelist) + block_length + 1, Blue);
              } else if (block_length > 0) { // ensure block isn't orphan
                  block[1] = (word)new_freelist;
                  new_freelist = block;
              }
            break;
          default:
      }

      // +1 to account for the fact that the header isn't included in the block  length
      block += block_length + 1;
  }

  freelist = new_freelist;

  if (!silent) printf("BS]");
}
```

