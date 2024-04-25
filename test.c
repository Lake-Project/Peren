#include <stdint.h>
#include <stdio.h>
int add(int a, int b)
{
    return a+ b;
}
// extern volatile unsigned int a;
extern char test(int a, int b);
int main(int argc, char const *argv[])
{
    char s = test(1, 21);
    printf("%d \n",s);
    // unsigned int d = -1 - 3;
    // float a = 'a';
    return 0;
}