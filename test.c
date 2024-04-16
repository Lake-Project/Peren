#include <stdio.h>

extern float testLink();
extern int global_var;

int main(int argc, char const *argv[])
{
    // printf("global_var: %d \n", global_var);
    float a = testLink();
    // assert(a == 12);
    printf("output: %f \n", a);
    // printf("global_var: %d \n", global_var);
    return 0;
}    
