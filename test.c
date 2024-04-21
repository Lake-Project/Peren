
#include <stdio.h>
extern int test(int a, int b);
int test_global = 20;
extern int global_var;
int add(int a, int b)
{
    return a + b;
}
int main(int argc, char const *argv[])
{
    int s = test(1, 1);
    printf("%d \n",s);
    return 0;
    // float d = 4.14;
}
