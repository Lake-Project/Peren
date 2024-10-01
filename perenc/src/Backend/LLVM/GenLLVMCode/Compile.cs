using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class Compile
{
    public static LLVMTypeRef ToLLVMType(Tokens type, LLVMContext Context)
    {
        if (type.tokenType == TokenType.Word)
        {
            return Context.GetType(type.buffer).Type;
        }

        return type.tokenType switch
        {
            TokenType.Char or TokenType.Byte or TokenType.Sbyte => LLVMTypeRef.Int8,
            TokenType.Int16 or TokenType.Uint16 => LLVMTypeRef.Int16,
            TokenType.Int or TokenType.Uint => LLVMTypeRef.Int32,
            TokenType.Int64 or TokenType.Ulong => LLVMTypeRef.Int64,

            TokenType.Float => LLVMTypeRef.Float,
            TokenType.Bool => LLVMTypeRef.Int1,
            TokenType.Void => LLVMTypeRef.Void,
            TokenType.String => LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0),
            _ => throw new Exception($"undefined {type.ToString()} type")
        };
    }
}