namespace LacusLLVM.SemanticAanylyzerVisitor.Linker;

public class BinSegment
{
    private List<byte> Bytes { get; set; }
    private byte To { get; set; }
    private byte From { get; set; }

    public BinSegment(List<byte> bytes, byte to, byte from)
    {
        Bytes = bytes;
        To = to;
        From = from;
        if ((To - From) < bytes.Count) return;
        int l = (To - From) - bytes.Count;
        for (int i = 0; i <= l; i++)
        {
            Bytes.Add(0x00);
        }
    }

    public byte GetByte(byte location)
    {
        return Bytes[To - location];
    }

    public void SetByte(byte location, byte value)
    {
        Bytes[To - location] = value;
    }

    public void PrintBytes()
    {
        int i = 0;
        Bytes.ForEach(n =>
        {
            // Console.WriteLine($"byte {i + from}: {n}");
            Console.WriteLine("byte: {0:X} value {1:x}", i + From, n);

            // Console.WriteLine(n);
            i++;
        });
    }
}

public class Link
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

    public static void LinkCode(string path)
    {
        File.ReadAllLines(path);
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Read the data from the file
                    while (fs.Position < fs.Length)
                    {
                        // Example of reading different types of data
                        int intData = br.ReadInt32();
                        double doubleData = br.ReadDouble();
                        byte byteData = br.ReadByte();

                        // Process the data
                    }
                }
            }
        }
        catch (Exception e)
        {
        }

        BinSegment header = new BinSegment(AddHeader(), 0x3f, 0x00);
        header.SetByte(0x3c, 0x80);
        BinSegment dosStub = new BinSegment(DosStub(), 0x7f, 0x40);
        dosStub.PrintBytes();
        // DateTime timestamp = DateTime.UnixEpoch.AddSeconds(seconds);
        BinSegment Header = new BinSegment(AddHeader(), 0x3f, 0x00);
        BinSegment PEHeader = new(new List<byte>()
        {
            0x50, 0x45, 0x00, 0x00, 0x64, 0x86
        }, 0, header.GetByte(0x3c));
    }
}