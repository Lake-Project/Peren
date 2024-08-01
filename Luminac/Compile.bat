@echo off
clang -g -c  Test.c -o test.o
dotnet run -- ./TestFiles/BinaryAddition.lm   -c --print-ir -o Output.o
clang test.o Output.o -o Output.exe
