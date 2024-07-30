// See https://aka.ms/new-console-template for more information


using Linker;

public class Link
{
    public static void LinkInvoke(string fileName)
    {
        LinkerRun.LinkCode(fileName);
    }
    // public static void Main(string[] args)
    // {
    //     Console.WriteLine("Hello, World!");
    // }
}