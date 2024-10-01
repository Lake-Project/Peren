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
    }

    public static PerenType TokenToPerenType(Tokens type, bool isConst, SemanticProgram program)
    {
        if (type.tokenType == TokenType.Word)
        {
            return program.Types.GetValue(type).Type;
        }

        return type.tokenType switch
        {
            TokenType.Int => new IntegerType(isConst),
            TokenType.Int16 => new IntegerType(isConst, Range.SixteenBit),
            TokenType.Int64 => new IntegerType(isConst, Range.SixtyFourBit),
            TokenType.Bool => new BoolType(isConst),
            TokenType.Ulong => new IntegerType(isConst, Range.SixtyFourBit, true),
            TokenType.Byte => new IntegerType(isConst, Range.EightBit, true),
            TokenType.Sbyte => new IntegerType(isConst, Range.EightBit),
            TokenType.Uint => new IntegerType(isConst, Range.ThirtyTwoBit, true),
            TokenType.Uint16 => new IntegerType(isConst, Range.SixteenBit, true),
            TokenType.Float => new FloatType(isConst),
            TokenType.Char => new CharType(isConst),
            TokenType.Void => new VoidType(),
            TokenType.String => new ArrayType(new CharType(false), isConst),
            _ => throw new Exception($"type{type.ToString()} doesnt exist")
        };
    }
}