namespace Linker;

public class Program
{
    public static void Main(string[] args)
    {
        LinkerRun.LinkCode("HelloWorld.o");
        // LinkerRun.LinkCode("Test.o");
    }
}