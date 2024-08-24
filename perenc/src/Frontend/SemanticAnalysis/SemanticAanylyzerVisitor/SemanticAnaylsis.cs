using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnaylsis
{
    public static void Init(PerenNode p)
    {
        var s = new SemanticAnayslisTopLevel();
        p.Visit(s);
        // n.ForEach(n => n.Visit(s));
    }

    public static PerenType TokenToPerenType(Tokens type, bool isConst, SemanticProgram program)
    {
        if (type.tokenType == TokenType.WORD)
        {
            return program.Types.GetValue(type).Type;
        }

        return type.tokenType switch
        {
            TokenType.INT => new IntegerType(isConst),
            TokenType.INT16 => new IntegerType(isConst, Range.SixteenBit),
            TokenType.INT64 => new IntegerType(isConst, Range.SixtyFourBit),
            TokenType.BOOL => new BoolType(isConst),
            TokenType.FLOAT => new FloatType(isConst),
            TokenType.CHAR => new CharType(isConst),
            TokenType.VOID => new VoidType(),
            TokenType.ULONG => new IntegerType(isConst, Range.SixtyFourBit, true),
            TokenType.BYTE => new IntegerType(isConst, Range.EightBit, true),
            TokenType.SBYTE => new IntegerType(isConst, Range.EightBit),
            TokenType.UINT => new IntegerType(isConst, Range.ThirtyTwoBit, true),
            TokenType.UINT_16 => new IntegerType(isConst, Range.SixteenBit, true),
            TokenType.STRING => new ArrayType(new CharType(false), isConst),
            _ => throw new Exception($"type{type.ToString()} doesnt exist")
        };
    }
}