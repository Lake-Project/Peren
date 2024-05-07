
// extern volatile unsigned int a;
int add(int a ){
    return a;
}
int main(int argc, char const *argv[])
{
//     int d = 1;
//    volatile int b = d + 1;
//     b = 10;
    // int s = test(1, 21);
    // int i = 0;
    // int a = argc;
    // for( i = 0; i < a; i++){
    //    add(i);
    //    i = i * 2;
    // }
    // i += 10;
    int d = 1;
    d = d+1;
    d += argc;
    return d;
    // int b = 0;
    // int a = 0; //a = 0
    // a = b; //a's dependcy graah: "b"
    // a = a + 1; // a: a, b
    // printf("%d\n",s);

    //good wya todo this

    //traverse the tree with perhaps a "Transform" method in Inode
    // and context Object

    // if traversal sees a var reference. it will add it to the dependecy graph 
    //if it sees a var declaration it will create a new dependecy gaph 
    //immediates get added as "immediate" 

    //ex: int a = 10 + b; //assuming b is declard already 
    //a's dependecy graph would be 
    // empty Node, node for b. with a var containing the depency graph for b
    //it builds the graph until it gets to return

    //at return. it goes through the list of vars/var references. if it doesnt turn up in the big traversal. it gets marked as dead code
    // and removed
    //
    // return i;
}