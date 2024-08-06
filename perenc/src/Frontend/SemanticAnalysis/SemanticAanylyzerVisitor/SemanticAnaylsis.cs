using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnaylsis
{
    public void SemanticEntry(ModuleNode n)
    {
        var s = new SemanticAnayslisTopLevel();
        n.Visit(s);
    }

    public static LacusType tokenToLacusType(Tokens type, bool isConst, SemanticProgram program)
    {
        if (type.tokenType == TokenType.WORD)
        {
            return program.Types.GetValue(type).Type;
        }

        return type.tokenType switch
        {
            TokenType.INT => new IntegerType(isConst, Range.ThirtyTwoBit),
            TokenType.INT16 => new IntegerType(isConst, Range.SixteenBit),
            TokenType.INT64 => new IntegerType(isConst, Range.SixtyFourBit),
            TokenType.BOOL => new BoolType(isConst, Range.OneBit),
            TokenType.FLOAT => new FloatType(isConst, Range.Float),
            TokenType.CHAR => new CharType(isConst, Range.EightBit),
            TokenType.VOID => new VoidType(),
            TokenType.ULONG => new IntegerType(isConst, Range.SixtyFourBit, true),
            TokenType.BYTE => new IntegerType(isConst, Range.EightBit, true),
            TokenType.SBYTE => new IntegerType(isConst, Range.EightBit),
            TokenType.UINT => new IntegerType(isConst, Range.ThirtyTwoBit, true),
            TokenType.UINT_16 => new IntegerType(isConst, Range.SixteenBit, true),
            TokenType.STRING => new ArrayType(new CharType(false, Range.EightBit), isConst),
            _ => throw new Exception($"type{type.ToString()} doesnt exist")
        };
    }
}