
#include <stdio.h>
#include <windows.h>

int add(int a, int b){
    return a+ b;
}
extern int test(int a, int b);
int main(int argc, char const *argv[])
{
    int s = test(1, 1);
    printf("%d \n",s);
    return 0;
}