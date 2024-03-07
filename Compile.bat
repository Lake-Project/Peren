@echo off
cd out
clang -g  output.ll 
start "" "%~dp0out\a.exe"