#include <stdio.h>
#include <assert.h>

extern int testLink();
extern int global_var;
int main(int argc, char const *argv[])
{
    int a = testLink();
    // assert(a == 12);
    printf("output: %d \n", a);
    printf("global_var: %d \n", global_var);
    return 0;
}
