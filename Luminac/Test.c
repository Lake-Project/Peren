#include <stdio.h>
extern char charat(int i);
int main()
{
    for(int i = 0; i < 11; i++)
    {
        printf("%c \n", charat(i));

    }
    // char* test = "a";
    // char a = test[0];
    // printf("%c \n", a);
}
