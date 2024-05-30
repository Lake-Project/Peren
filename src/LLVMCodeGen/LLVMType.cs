using Lexxer;
using LLVMSharp.Interop;

public struct StructDimension
{
    public Dictionary<string, LLVMTypeRef> GetStruct;

    public StructDimension()
    {
        GetStruct = new();
    }
}

public class LLVMType
{
    private List<StructDimension> Types = new();

    public LLVMTypeRef GetStruct(Tokens name)
    {
        for (int i = 0; i < Types.Count; i++)
        {
            if (Types[i].GetStruct.ContainsKey(name.buffer))
            {
                return Types[i].GetStruct[name.buffer];
            }
        }
        throw new TypeDoesntExistException(
            $"Type {name.buffer} doesnt exist on line {name.GetLine() + 1}"
        );
    }

    public LLVMTypeRef TokenToLLVMRef(Tokens type)
    {
        if (type.tokenType == TokenType.STRUCT)
        {
            return GetStruct(type);
        }
        return type.tokenType switch
        {
            TokenType.INT => LLVMTypeRef.Int32,
            TokenType.FLOAT => LLVMTypeRef.Float,
            TokenType.CHAR => LLVMTypeRef.Int8,
            TokenType.BOOL => LLVMTypeRef.Int1,
            _
              => throw new TypeDoesntExistException(
                  $"Type {type.buffer} doesnt exist on line {type.GetLine() + 1}"
              )
        };
    }
}

public class TypeDoesntExistException : Exception
{
    public TypeDoesntExistException(string message) : base(message) { }
}
