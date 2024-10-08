using System.Text;

namespace Linker;

public class LinkerRun
{
    public static List<byte> AddHeader()
    {
        List<byte> Header = new()
        {
            0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00,
            0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00,
            0x80, 0x00, 0x00
        };
        return Header;
    }

    public static List<byte> DosStub()
    {
        List<byte> DosMode = new()
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
        return DosMode;
    }
    // public static byte GetBegin()

    public static List<byte> TextSection(List<byte> p)
    {
        List<byte> TextSection = new();
        uint TextSectionEnd = (uint)(p[0x2d] << 24 | p[0x2c] << 16 | p[0x2b] << 8 | p[0x2a] & 255);
        uint TextSectionBegin = (uint)(p[0x29] << 24 | p[0x28] << 16 | p[0x27] << 8 | p[0x26] & 255);
        TextSectionBegin = (TextSectionBegin >> 16) | (TextSectionBegin << 16);
        TextSectionEnd = (TextSectionEnd >> 16) | (TextSectionEnd << 16);

        for (uint i = TextSectionBegin; i < TextSectionEnd; i++)
        {
            TextSection.Add(p[(int)i]);
        }


        Console.WriteLine("text begin: {0:x}", TextSectionBegin);
        Console.WriteLine("text end: {0:x}", TextSectionEnd);
        return TextSection;
    }

    /// <summary>
    ///
    /// Addresses
    /// Object file tells you how far the sections are
    /// .text: start: 0x29 emd: 0x2c
    /// 
    /// </summary>
    /// <param name="path"></param>
    public static void LinkCode(string path)
    {
        // int p1 = File.ReadAllLines(path);
        List<byte> file = File.ReadAllBytes(path).ToList();
        List<byte> textSection = TextSection(file);

        // List<byte> Get

        // textSection.ForEach(n => Console.WriteLine("{0:x}",n));
        // p.ForEach(n => { Console.WriteLine("byte: {0:X} ", n); });
        // Console.WriteLine("{0:x}", p[0x2c]);
        // p[0x2a] = 0xa;

        var header = new BinSegment(AddHeader(), 0x3f, 0x00);
        header.SetByte(0x3c, 0x80);
        var dosStub = new BinSegment(DosStub(), 0x7f, 0x40);
        // dosStub.PrintBytes();
        // DateTime timestamp = DateTime.UnixEpoch.AddSeconds(seconds);
        var Header = new BinSegment(AddHeader(), 0x3f, 0x00);
        var PEHeader = new BinSegment(new List<byte>()
        {
            0x50, 0x45, 0x00, 0x00, 0x64, 0x86
        }, 0, header.GetByte(0x3c));
    }
}