#include <stdio.h>
extern int f(int a, int b);

long add(int a, int b)
{
    return a + b;
}

int main()
{
    long a = f(1, 1);
    printf("out: %d \n", a);
}
