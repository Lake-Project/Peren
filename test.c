#include <stdio.h>

extern int testLink();
extern int global_var;

int main(int argc, char const *argv[])
{
    printf("global_var: %d \n", global_var);
    int a = testLink();
    // assert(a == 12);
    printf("output: %d \n", a);
    printf("global_var: %d \n", global_var);
    return 0;
    // a();
    return 0;
}
