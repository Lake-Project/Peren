@echo off
clang -c  test.c -o test.o
dotnet run -- test.lk -c -o Output.o

clang test.o Output.o -o Out.exe
Out.exe
