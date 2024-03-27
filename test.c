#include <stdio.h>

extern int testLink();

int main(int argc, char const *argv[])
{
    int a = testLink();
    printf("output: %d \n", a);
    return 0;
}
