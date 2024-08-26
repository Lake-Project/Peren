using System.Text;

namespace Linker.Elf;

public struct Elf(Elf64_Ehdr hdr)
{
    public void print()
    {
        Console.WriteLine(ASCIIEncoding.Default.GetString(hdr.e_ident));
        Console.WriteLine("32  or 64 bit (0 invalid, 1 32 bit, 2 64 bit {0:X}", hdr.e_class);
        Console.WriteLine("LSB or MSB {0:x} 0 or 1", hdr.e_data);
        Console.WriteLine("OS ABI {0:x}", hdr.OS_ABI);
        Console.WriteLine($"Machine: {hdr.e_machine switch {
            0x03 => "intel x86",
            0x08 => "MIPS",
            0x28 => "ARM",
            0x3E => "AMD64",
            0xB7 => "ARM v8",
            0xF3 => "RISC-V",
            _ => throw new Exception("unsupported machine type")
        }}");
        // Console.WriteLine("Machine {0:x}");
        Console.WriteLine("Machine {0:x}", hdr.e_type);
    }
}

public class ElfParse
{
    private List<byte> Raw { get; set; }

    public string FilePath { get; set; }

    public ElfParse(string filePath)
    {
        Raw = File.ReadAllBytes(filePath).ToList();
        FilePath = filePath;
    }
    
    private Dictionary<string, List<byte>> GetElfSections(Elf64_Ehdr header)
    {
        Dictionary<string, List<byte>> section = new();
        ulong ptr = header.e_shoff;
        for (int i = 0; i < header.e_shnum; i++)
        {
            Elf64SectionHeader b = Util.GetSection<Elf64SectionHeader>(Raw, (uint)ptr, 64);
            ptr += 64;
            List<byte> Section = new();
            if (b.sh_offset != 0x00)
            {
                for (
                    ulong sectionIdx = b.sh_offset;
                    sectionIdx < b.sh_offset + b.sh_size;
                    sectionIdx++
                )
                {
                    Section.Add(Raw[(int)sectionIdx]);
                }

                Section.ForEach(n =>
                {
                    Console.Write("0x{0:x}-", n);
                });
                
                Console.WriteLine("");
                Console.WriteLine("");

            }
               
        }

        return section;
    }

    public Elf GetElf()
    {
        using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);
        var header = Util.GetSection<Elf64_Ehdr>(reader);
        ulong ptr = header.e_shoff;
        Console.WriteLine("{0:x}", header.e_shoff);
        GetElfSections(header);
        
        return new Elf(header);
    }
}