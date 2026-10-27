
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
// This is the version that doesn't merge adjacent blocks
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

Since we're rebuilding the whole list, we can improve the algorithm by also
merging blue blocks thus reducing memory fragmentation even more.

This is rather simple to implement. Since we always insert into the front of
the new freelist, the head of the new freelist will be equal to the previous
block we added.

We simply check that the length of the block at the head of the list plus 1 to
account for the header yields the block we're currently inspecting. If it does,
then we know that the blocks are adjacent and we merge them by simply updating
the length in the header of the block in the freelist.

If they aren't adjacent, then we just do what we did earlier and append the
block to the front of the list.

Finally there's an edge case for orphans. If we have an orphan and the previous
block isn't adjacent to the orphan, then we have to actually look to the right
of the orphan to see if there's somethin we can merge the orphan with. If there
is, we merge it thus making it no longer an orphan. Otherwise it just remains
an orphan.


```c
// This is the version that merges adjacent blocks
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
              if(new_freelist != NULL && new_freelist + Length(*new_freelist) + 1 == block) {
                  // This current block is adjacent to the previous block we looked at.
                  // So we just merge the previous blcok we the current block.
                  *new_freelist = mkheader(0, Length(*new_freelist) + block_length + 1, Blue);
              } else if (block_length > 0) { // ensure block isn't orphan
                  // This is a non-orphan block and non-adjacent.
                  // So we just append it to the front of the list.
                  block[1] = (word)new_freelist;
                  new_freelist = block;
              } else if (block + 1 < afterHeap && Color(*(block + 1)) != Black) { // Orphan case
                  // In the edge case where where we have an orphan and we don't have a previous block to merge it with.
                  // Therefore we need to lookahead and see if there's an adjacent free block to the right of it,
                  // if there is, then merge with the right block.
                  block_length = Length(*block) + Length(*(block + 1)) + 1;
                  *block = mkheader(0, block_length + 1, Blue);
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



Finally we make sure it all works. Here's the output of each test case.
Note that we're running this command from within the MicroVM directory.

Test case `ex30`:
```
$ ./microvm ../ListC/ListcEx/ex30.out 1000
1000 999 998 997 996 995 994 993 992 991 990 989 988 987 986 985 984 983 982 981 980 979 978 977 976 975 974 973 972 971 970 969 968 967 966 965 964 963 962 961 960 959 958 957 956 955 954 953 952 951 950 949 948 947 946 945 944 943 942 941 940 939 938 937 936 935 934 933 932 931 930 929 928 927 926 925 924 923 922 921 920 919 918 917 916 915 914 913 912 911 910 909 908 907 906 905 904 903 902 901 900 899 898 897 896 895 894 893 892 891 890 889 888 887 886 885 884 883 882 881 880 879 878 877 876 875 874 873 872 871 870 869 868 867 866 865 864 863 862 861 860 859 858 857 856 855 854 853 852 851 850 849 848 847 846 845 844 843 842 841 840 839 838 837 836 835 834 833 832 831 830 829 828 827 826 825 824 823 822 821 820 819 818 817 816 815 814 813 812 811 810 809 808 807 806 805 804 803 802 801 800 799 798 797 796 795 794 793 792 791 790 789 788 787 786 785 784 783 782 781 780 779 778 777 776 775 774 773 772 771 770 769 768 767 766 765 764 763 762 761 760 759 758 757 756 755 754 753 752 751 750 749 748 747 746 745 744 743 742 741 740 739 738 737 736 735 734 733 732 731 730 729 728 727 726 725 724 723 722 721 720 719 718 717 716 715 714 713 712 711 710 709 708 707 706 705 704 703 702 701 700 699 698 697 696 695 694 693 692 691 690 689 688 687 686 685 684 683 682 681 680 679 678 677 676 675 674 673 672 671 670 669 668 GC[M,BS]Heap: 2 blocks (997 words); of which 1 free (995 words, largest 995 words); 1 orphans
667 666 665 664 663 662 661 660 659 658 657 656 655 654 653 652 651 650 649 648 647 646 645 644 643 642 641 640 639 638 637 636 635 634 633 632 631 630 629 628 627 626 625 624 623 622 621 620 619 618 617 616 615 614 613 612 611 610 609 608 607 606 605 604 603 602 601 600 599 598 597 596 595 594 593 592 591 590 589 588 587 586 585 584 583 582 581 580 579 578 577 576 575 574 573 572 571 570 569 568 567 566 565 564 563 562 561 560 559 558 557 556 555 554 553 552 551 550 549 548 547 546 545 544 543 542 541 540 539 538 537 536 535 534 533 532 531 530 529 528 527 526 525 524 523 522 521 520 519 518 517 516 515 514 513 512 511 510 509 508 507 506 505 504 503 502 501 500 499 498 497 496 495 494 493 492 491 490 489 488 487 486 485 484 483 482 481 480 479 478 477 476 475 474 473 472 471 470 469 468 467 466 465 464 463 462 461 460 459 458 457 456 455 454 453 452 451 450 449 448 447 446 445 444 443 442 441 440 439 438 437 436 435 434 433 432 431 430 429 428 427 426 425 424 423 422 421 420 419 418 417 416 415 414 413 412 411 410 409 408 407 406 405 404 403 402 401 400 399 398 397 396 395 394 393 392 391 390 389 388 387 386 385 384 383 382 381 380 379 378 377 376 375 374 373 372 371 370 369 368 367 366 365 364 363 362 361 360 359 358 357 356 355 354 353 352 351 350 349 348 347 346 345 344 343 342 341 340 339 338 337 336 GC[M,BS]Heap: 3 blocks (997 words); of which 2 free (995 words, largest 992 words); 0 orphans
335 334 333 332 331 330 329 328 327 326 325 324 323 322 321 320 319 318 317 316 315 314 313 312 311 310 309 308 307 306 305 304 303 302 301 300 299 298 297 296 295 294 293 292 291 290 289 288 287 286 285 284 283 282 281 280 279 278 277 276 275 274 273 272 271 270 269 268 267 266 265 264 263 262 261 260 259 258 257 256 255 254 253 252 251 250 249 248 247 246 245 244 243 242 241 240 239 238 237 236 235 234 233 232 231 230 229 228 227 226 225 224 223 222 221 220 219 218 217 216 215 214 213 212 211 210 209 208 207 206 205 204 203 202 201 200 199 198 197 196 195 194 193 192 191 190 189 188 187 186 185 184 183 182 181 180 179 178 177 176 175 174 173 172 171 170 169 168 167 166 165 164 163 162 161 160 159 158 157 156 155 154 153 152 151 150 149 148 147 146 145 144 143 142 141 140 139 138 137 136 135 134 133 132 131 130 129 128 127 126 125 124 123 122 121 120 119 118 117 116 115 114 113 112 111 110 109 108 107 106 105 104 103 102 101 100 99 98 97 96 95 94 93 92 91 90 89 88 87 86 85 84 83 82 81 80 79 78 77 76 75 74 73 72 71 70 69 68 67 66 65 64 63 62 61 60 59 58 57 56 55 54 53 52 51 50 49 48 47 46 45 44 43 42 41 40 39 38 37 36 35 34 33 32 31 30 29 28 27 26 25 24 23 22 21 20 19 18 17 16 15 14 13 12 11 10 9 8 7 6 5 4 GC[M,BS]Heap: 3 blocks (997 words); of which 2 free (995 words, largest 989 words); 0 orphans
3 2 1 
Result value: 0 
Used 0 cpu milli-seconds
Number of GC: 3

```

Test case `ex35`:

Note that the HEAPSIZE is too big to trigger a garbage collection, so please update HEAPSIZE to 1000 to trigger a GC.

```
$ ./microvm ../ListC/ListcEx/ex35.out 352612536
33 33 GC[M,BS]Heap: 4 blocks (995 words); of which 1 free (989 words, largest 989 words); 1 orphans
44 44 
Result value: 352612535 
Used 0 cpu milli-seconds
Number of GC: 1
```


Test case `ex36`:

```
$ ./microvm ../ListC/ListcEx/ex36.out 352612536
1 GC[M,BS]Heap: 3 blocks (996 words); of which 1 free (992 words, largest 992 words); 1 orphans
1 
Result value: 352612535 
Used 1 cpu milli-seconds
Number of GC: 1

```

