#include <stdint.h>
#include <stdio.h>
int add(int a, int b)
{
    return a+ b;
}
// extern volatile unsigned int a;
extern int test(int a, int b);
int main(int argc, char const *argv[])
{
    int s = test(1, 1);
    printf("%d \n",s);
    // unsigned int d = -1 - 3;
    return 0;
}