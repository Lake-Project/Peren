#include <stdio.h>
#include <assert.h>

extern int testLink();

int main(int argc, char const *argv[])
{
    int a = testLink();
    // assert(a == 12);
    printf("output: %d \n", a);
    return 0;
}
