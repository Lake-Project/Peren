#include <stdio.h>
extern int f(int x);
int main()
{
    int a = f(4);
    printf("out: %d \n",a);
}