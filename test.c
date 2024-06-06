#include <stdio.h>


extern int add(int a, int b);

int main(int argc, char const* argv[])
{
    printf("hello owlrd");
    int a = add(1, 1);
    printf("result: %d \n", a);
    return 0;
}
