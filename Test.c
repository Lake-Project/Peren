#include <stdio.h>
extern int factorial(int a);

long add(int a, int b)
{
    return a + b;
}

int main()
{
    long a = factorial(1);
    printf("out: %d \n", a);
}
