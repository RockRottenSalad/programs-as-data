
#import "@preview/finite:0.5.1": automaton

= Exercise 3.2
A regular expression that recognizes all sequences consisting of _a_ and _b_ where two _a_'s are always seperated by at least one _b_ can be written as:
$ (a?b)"*"a? $

Below is an NFA of the regular expression:
#image("first.drawio.png")

Below is the NFA is converted into its corresponding DFA:
#image("second.drawio (1).png")

= Exercise 3.3
`let z = (17) in z + 2 * 3 end EOF`
```
Main
=> Expr EOF                                                                (rule A)
=> LET NAME EQ Expr IN Expr END EOF                                        (rule F)
=> LET NAME EQ EXPR IN EXPR PLUS EXPR END EOF                              (rule H)
=> LET NAME EQ EXPR IN EXPR PLUS EXPR TIMES EXPR END EOF                   (rule G)
=> LET NAME EQ EXPR IN EXPR PLUS EXPR TIMES CSTINT END EOF                 (rule C)
=> LET NAME EQ EXPR IN EXPR PLUS CSTINT TIMES CSTINT END EOF               (rule C)
=> LET NAME EQ EXPR IN NAME PLUS CSTINT TIMES CSTINT END EOF               (rule B)
=> LET NAME EQ LPAR Expr RPAR IN NAME PLUS CSTINT TIMES CSTINT END EOF     (rule E)
=> LET NAME EQ LPAR CSTINT RPAR IN NAME PLUS CSTINT TIMES CSTINT END EOF   (rule C)
```

= Exercise 3.4
#image("derivationtree.drawio.png")