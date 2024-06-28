#include <stdio.h>

extern char* helloWorld();


int main()
{
   char* f = helloWorld();
    printf("%s \n", f);
}
