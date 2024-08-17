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
}