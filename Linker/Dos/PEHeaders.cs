using System.Runtime.InteropServices;

namespace Linker.Dos;
/// <summary>
/// https://wiki.osdev.org/COFF
/// https://wiki.osdev.org/MZ
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MZHeader
{
    ushort MagicNumber; // Magic number (must be 'MZ' or 0x5A4D)
    ushort BytesOnLastPage; // Bytes on the last page of the file
    ushort PagesInFile; // Total number of 512-byte pages in the file
    ushort RelocationCount; // Number of entries in the relocation table
    ushort HeaderSizeInParagraphs; // Size of the header in 16-byte paragraphs
    ushort MinimumExtraParagraphs; // Minimum number of additional paragraphs needed
    ushort MaximumExtraParagraphs; // Maximum number of additional paragraphs allowed
    ushort InitialSS; // Initial (relative) SS register value
    ushort InitialSP; // Initial SP register value
    ushort Checksum; // Checksum of the file
    ushort InitialIP; // Initial IP register value
    ushort InitialCS; // Initial (relative) CS register value
    ushort RelocationTableOffset; // Offset to the relocation table
    ushort OverlayNumber; // Overlay number (used for overlay management)

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    ushort[] ReservedWords; // Reserved; must be 0

    ushort OEMIdentifier; // OEM identifier (used by e_oeminfo)
    ushort OEMInformation; // OEM information; e_oemid specific
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    ushort[] AdditionalReservedWords; // Additional reserved words; must be 0

    uint PEHeaderOffset; // Offset to the PE header (new EXE header)
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Pe_Header
{
    uint mMagic; // PE\0\0 or 0x00004550
    ushort mMachine;
    ushort mNumberOfSections;
    uint mTimeDateStamp;
    uint mPointerToSymbolTable;
    uint mNumberOfSymbols;
    short mSizeOfOptionalHeader;
    uint mCharacteristics;
};
[StructLayout(LayoutKind.Sequential, Pack = 1)]


public struct DosStub()
{
    public static byte[] Dos_stub =
    {
        0x0e, 0x01f, 0xba, 0x0e, 0x00, 0xb4, 0x09, 0xcd,
        0x21, 0xb8, 0x01, 0x4c, 0xcd, 0x21, 0x54, 0x68,
        0x69, 0x73, 0x20, 0x70, 0x72, 0x6F, 0x67, 0x72,
        0x61, 0x6D, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F,
        0x74, 0x20, 0x62, 0x65, 0x20, 0x72, 0x75, 0x6E,
        0x20, 0x69, 0x6E, 0x20, 0x44, 0x4F, 0x53, 0x20,
        0x6d, 0x6f, 0x64, 0x65, 0x2e, 0x0d, 0x0d, 0x0a,
        0x24
    };
    

};