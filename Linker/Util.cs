using System.Runtime.InteropServices;

namespace Linker;

public class Util
{
    public static T GetSection<T>(BinaryReader reader)
    {
        // using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        // using var reader = new BinaryReader(stream);
        // reader.
        // while ()
        // {
        //     
        // }
        // reader.PeekChar();
        byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();
        return theStructure;
    }

    public static  T GetSection<T>(List<byte> raw, uint ptr, int size)
    {
        byte[] bytes = new byte[size];
        int idx = 0;
        for (int i = (int)ptr; i < ptr + size; i++)
        {
            
            bytes[idx] = raw[i];
            idx++;
        }
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();
        return theStructure;
    }
}