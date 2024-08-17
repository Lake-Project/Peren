namespace Linker.Elf;


public struct Elf { }

public class ElfParse
{
    private List<byte> Raw { get; set; }

    public string FilePath { get; set; }

    public ElfParse(string filePath)
    {
        Raw = File.ReadAllBytes(filePath).ToList();
        FilePath = filePath;
    }

    public Elf GetElf()
    {
        return new Elf();
    }
}
