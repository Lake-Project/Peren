using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;

namespace Linker;

// COFF Header structure
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Coff_Hdr
{
    public ushort Machine;
    public ushort NumberOfSections;
    public uint TimeDateStamp;
    public uint PointerToSymbolTable;
    public uint NumberOfSymbols;
    public ushort SizeOfOptionalHeader;
    public ushort Characteristics;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
// [MarshalAs(UnmanagedType.Struct, SizeConst = 52)]
public unsafe struct CoffSectionHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Name; // 8 bytes

    public uint VirtualSize; // 4 bytes (used in PE files)
    public uint VirtualAddress; // 4 bytes
    public uint SizeOfRawData; // 4 bytes
    public uint PointerToRawData; // 4 bytes
    public uint PointerToRelocations; // 4 bytes
    public uint PointerToLinenumbers; // 4 bytes
    public ushort NumberOfRelocations; // 2 bytes
    public ushort NumberOfLinenumbers; // 2 bytes
    public uint Characteristics; // 4 bytes
}

public class DosLinker
{
    
    
    public static Dictionary<string, List<byte>> GetSections(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Dictionary<string, List<byte>> section = new();

        List<byte> raw = File.ReadAllBytes(filePath).ToList();
        List<CoffSectionHeader> sectionHeaders = new();
        using var reader = new BinaryReader(stream);

        Coff_Hdr a = Util.GetSection<Coff_Hdr>(reader);
        List<byte> SymbolTable = new();
        for (uint idx = a.PointerToSymbolTable; idx < (a.PointerToSymbolTable + a.NumberOfSymbols); idx += 18)
        {
            
            for (uint idy = idx; idy < idx +  18; idy++)
            {
                SymbolTable.Add(raw[(int)idy]);
            }
        }

        for (int i = 0; i < a.NumberOfSections; i++)
        {
            CoffSectionHeader b = Util.GetSection<CoffSectionHeader>(reader);
            List<byte> Section = new();
            
            for (uint idx = b.PointerToRawData; idx < b.PointerToRawData + b.SizeOfRawData; idx++)
            {
                Section.Add(raw[(int)idx]);
            }

            section.Add(Encoding.Default.GetString(b.Name), Section);
        }

        section.ToList().ForEach(n =>
        {
            Console.WriteLine($"section: {n.Key} data: {BitConverter.ToString(n.Value.ToArray())}");
        });
        // Console.W
        Console.WriteLine("{0:X}", a.PointerToSymbolTable);
        Console.WriteLine("{0:X}", a.NumberOfSymbols);

        return section;
    }
}