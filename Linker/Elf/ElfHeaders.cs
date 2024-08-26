using System.Runtime.InteropServices;

namespace Linker.Elf;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Elf64_Ehdr
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] e_ident; // Magic number and other info

    public byte e_class; //64 or 32
    public byte e_data; //LSB or MSB
    public byte e_ver;
    public byte OS_ABI;
    public byte ABI_VER; //unused just adding this for proper padding

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    public byte[] padding; // padding

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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Elf64SectionHeader
{
    public uint sh_name; // Section name (string table index 0x01f98)
    public uint sh_type; // Section type
    public ulong sh_flags; // Section flags
    public ulong sh_addr; // Section virtual address at execution
    public ulong sh_offset; // Section file offset
    public ulong sh_size; // Section size in bytes
    public uint sh_link; // Link to another section
    public uint sh_info; // Additional section information
    public ulong sh_addralign; // Section alignment
    public ulong sh_entsize; // Entry size if section holds a table
}