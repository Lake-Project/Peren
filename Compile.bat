@echo off
clang -g -c  Test.c -o test.o
dotnet run -- test.lk -c -o Output.o

clang test.o Output.o -o Output.exe
