clang -c test.c -o out/test.o

cd out
clang -c  output.ll -o output.o
clang test.o output.o -o a.exe
./a.exe