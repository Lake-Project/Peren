namespace Linker;

using System;
using System.IO;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
unsafe public struct Elf64_Ehdr
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] e_ident; // Magic number and other info

    public ushort e_type; // Object file type
    public ushort e_machine; // Architecture
    public uint e_version; // Object file version
    public ulong e_entry; // Entry point virtual address
    public ulong e_phoff; // Program header table file offset
    public ulong e_shoff; // Section header table file offset
    public uint e_flags; // Processor-specific flags
    public ushort e_ehsize; // ELF header size in bytes
    public ushort e_phentsize; // Program header table entry size
    public ushort e_phnum; // Program header table entry count
    public ushort e_shentsize; // Section header table entry size
    public ushort e_shnum; // Section header table entry count
    public ushort e_shstrndx; // Section header string table index
}

public class ElfLinker
{
    public static Elf64_Ehdr DeserializeElfHeader(string filePath)
    {
        Elf64_Ehdr header = new Elf64_Ehdr();
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);
        Elf64_Ehdr a = Util.GetSection<Elf64_Ehdr>(reader);
        a.e_ident.ToList().ForEach(n => Console.WriteLine("byte: {0:X}", n));
        return header;
    }
}