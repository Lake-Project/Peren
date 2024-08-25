using Linker.Dos;

namespace Linker;

public class Program
{
    public static void Main(string[] args)
    {
        // ElfLinker.DeserializeElfHeader("Elf.o");
        // LinkerRun.LinkCode("HelloWorld.o");
        new CoffParser("./Binaries/HelloWorld.o").GetCoff().print();
        
        // DosLinker.GetSections("Test.o");
        // var b = ElfHeaderSearlize.DeserializeElfHeader("Elf.o");
        // b.e_ident.ToList().ForEach(n => Console.WriteLine("{0:X}", n));
    }
}