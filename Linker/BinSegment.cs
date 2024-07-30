namespace Linker;

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