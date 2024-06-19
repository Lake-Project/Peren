#include <stdio.h>
extern int factorial(int n);

long add(int a, int b)
{
    return a + b;
}

int main()
{
    int a = factorial(0);
    printf("out: %d \n", a);
}
