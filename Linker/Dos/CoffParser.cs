using System.Text;

namespace Linker.Dos;

public struct Coff(Coff_Hdr header, Dictionary<string, List<byte>> sections, List<SymbolTable> symbolTable)
{
    public Coff_Hdr Header { get; set; } = header;
    public Dictionary<string, List<byte>> Sections { get; set; } = sections;
    public List<SymbolTable> SymbolTable { get; set; } = symbolTable; //idk

    public void print()
    {
        Console.WriteLine("Header COFF x64 amd");
        Console.WriteLine("");
        Console.WriteLine("Machine 0x{0:x}", Header.Machine);
        Console.WriteLine($"Number Of Sections {Header.NumberOfSections}");
        Console.WriteLine($"TimeDate: {Header.TimeDateStamp}");
        Console.WriteLine("Pointer to symbol table 0x{0:x}", Header.PointerToSymbolTable);
        Console.WriteLine($"Number of symbols: {Header.NumberOfSymbols}");
        Console.WriteLine($"characteristics {Header.Characteristics}");
        Console.WriteLine("");
        Console.WriteLine("sections");
        Console.WriteLine("");
        Sections
            .ToList()
            .ForEach(n =>
            {
                Console.WriteLine(
                    $"section: {n.Key} data: {BitConverter.ToString(n.Value.ToArray())}"
                );
            });

        Console.WriteLine("");
        Console.WriteLine("Symbol table bytes");
        Console.WriteLine("");
        SymbolTable
            .ToList()
            .ForEach(n =>
            {
                Console.WriteLine($"Section {ASCIIEncoding.Default.GetString(n.Name)}");
                Console.WriteLine(
                    "data: {0:X}",n.Value
                );
                Console.WriteLine("section num: {0:X}",n.SectionNumber );
                Console.WriteLine("");
            });
        // Console.WriteLine("");
    }
}

public class CoffParser
{
    private List<byte> Raw { get; set; }

    public string FilePath { get; set; }

    // public FileStream Stream { get; set; }

    public CoffParser(string filePath)
    {
        Raw = File.ReadAllBytes(filePath).ToList();
        FilePath = filePath;
        // Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    private Dictionary<string, List<byte>> GetCoffSections(Coff_Hdr header, BinaryReader reader)
    {
        Dictionary<string, List<byte>> section = new();

        for (int i = 0; i < header.NumberOfSections; i++)
        {
            Coff_Section_Hdr b = Util.GetSection<Coff_Section_Hdr>(reader);
            List<byte> Section = new();
            if (b.PointerToRawData != 0x00)
                for (
                    uint sectionIdx = b.PointerToRawData;
                    sectionIdx < b.PointerToRawData + b.SizeOfRawData;
                    sectionIdx++
                )
                {
                    Section.Add(Raw[(int)sectionIdx]);
                }

            section.Add(Encoding.Default.GetString(b.Name), Section);
        }

        return section;
    }

    public Coff GetCoff()
    {
        Dictionary<string, List<byte>> section = new();
        using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);
        Coff_Hdr header = Util.GetSection<Coff_Hdr>(reader);
        // Coff_Hdr header = Util.GetSection<Coff_Hdr>(Raw, 0);
        // List<byte> SymbolTable = new();
        List<SymbolTable> symbolTables = new();
        Console.WriteLine("Machine 0x{0:x}", header.Machine);
        uint ptr = header.PointerToSymbolTable;
        for (int i = 0; i < header.NumberOfSymbols; i++)
        {
            var b = Util.GetSection<SymbolTable>(Raw, ptr, 18);
            symbolTables.Add(b);

            ptr += 18;
        }


        return new(header, GetCoffSections(header, reader), symbolTables);
    }
}