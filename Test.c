#include <stdio.h>
extern int testFunction(int n);
int main()
{
    int a = testFunction(2);
    printf("out: %d \n",a);
}