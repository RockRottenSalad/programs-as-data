# Exercise 10.1
> To understand how the abstract machine and the garbage collector
work and how they collaborate, answer these questions:
> 
> (i) Write 3–10 line descriptions of how the abstract machine executes each of the
following instructions:

- `ADD`: Untags the two integers at the top of the stack. Computes the sum of them, tags the result and pushes it to the stack.
- `CSTI i`: Tags the value `i` and pushes it to the stack.
- `NIL`: Pushes NILVALUE to the stack. The different between `CstI 0` and `NIL`, is that `NILVALUE` is not tagged.
- `IFZERO`: Gets the value just underneath the instruction. If the value is an int, it untags it and checks if it is 0. If it is zero, it jumps to some other part of the code by modifying pc, otherwise it simply increases pc.
- `CONS`: It allocates a cons cell on the heap called `p`. It makes the values of `p` be the two top stack values, and then leaves a pointer to the heap allocated cons cell on the stack.
- `CAR`: Casts the pointer stack value pointer to a word pointer, gets the first element of the cons cell and leaves it on the stack.
- `SETCAR`: The stacks cointains a pointer to a cons cell and a value. It then sets the value of the first component to being the value. It pops of the value from the stack leaving only the pointer.

> (ii)  Describe the result of applying each C macro Length, Color and Paint from
Sect. 10.7.4 to a block header ttttttttnnnnnnnnnnnnnnnnnnnnnngg, that
is, a 32-bit word, as described in the source code comments

- `Length`: Discards the two color bits by bit shifting the header to bits to the right, then ands with a bit mask that keeps all the length bits.
- `Color`: It does logical and with the header and `3`, which keeps the `2` color bits, and returns it.
- `Paint`: Clears the color bits by doing `hdr & 0xFFFFFFFFFFFFFFFC` leaving the res of the header untouched. Then it ands the result with the color, to set the color bits to the intended color.


> (iii) When does the abstract machine, or more precisely, its instruction interpretation
loop, call the allocate(…) function? Is there any other interaction between the
abstract machine (also called the mutator) and the garbage collector?

When it encounters a con cell or a `HEAPALLOC`.

> (iv) In what situation will the garbage collector’s collect(…) function be called?

When we try to allocate something, but there is not space in the heap. We then do a garbage collect and try again.

# Exercise 10.2

# Exercise 10.3