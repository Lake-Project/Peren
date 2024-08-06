using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class Compile
{
    public static LLVMTypeRef ToLLVMType(Tokens type, LLVMContext Context)
    {
        if (type.tokenType == TokenType.WORD)
        {
            return Context.types.Get(type.buffer).Type;
        }

        return type.tokenType switch
        {
            TokenType.INT or TokenType.UINT => LLVMTypeRef.Int32,
            TokenType.INT16 or TokenType.UINT_16 => LLVMTypeRef.Int16,
            TokenType.INT64 or TokenType.ULONG => LLVMTypeRef.Int64,

            TokenType.FLOAT => LLVMTypeRef.Float,
            TokenType.BOOL => LLVMTypeRef.Int1,
            TokenType.CHAR or TokenType.BYTE or TokenType.SBYTE => LLVMTypeRef.Int8,
            TokenType.VOID => LLVMTypeRef.Void,
            TokenType.STRING => LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0),
            _ => throw new Exception($"undefined {type.ToString()} type")
        };
    }

    public static LLVMVar GetVar(LLVMContext context, string name)
    {
        return context.globalVars.Values.ContainsKey(name) ? context.globalVars.Get(name) : context.vars.Get(name);
    }
}