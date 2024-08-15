using System.Runtime.InteropServices;

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
public struct CoffSectionHeader
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
}