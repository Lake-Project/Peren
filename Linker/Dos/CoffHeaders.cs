namespace Linker.Dos;

using System.Runtime.InteropServices;

/// <summary>
/// designed for DOS files COFF HDR is the 1st 20 bytes of your raw binary
/// </summary>
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

/// <summary>
/// this is for .text and .data sections it takes up 40 bytes for how many sections exist
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Coff_Section_Hdr
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Name;

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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SymbolTable
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Name;    
    public long Value; // technically, the effective type of this field 

// is determined by values of the two following fields 
    public short SectionNumber;
    public ushort Type; // a pair of one-byte enumerations
    public byte NumberOfAuxSymbols;
    public byte sclass;

}