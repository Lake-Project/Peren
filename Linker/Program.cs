namespace Linker;

public class Program
{
    public static void Main(string[] args)
    {
        // LinkerRun.LinkCode("HelloWorld.o");
        LinkerRun.LinkCode("Test.o");
        // var b = ElfHeaderSearlize.DeserializeElfHeader("Elf.o");
        // b.e_ident.ToList().ForEach(n => Console.WriteLine("{0:X}", n));
    }
}