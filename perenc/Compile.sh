!#/bin/bash
gcc -g -c  Test.c -o test.o
dotnet run -- test.pn   -c --print-ir -o Output.o
gcc test.o Output.o -o Output.out
