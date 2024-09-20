using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class CharType(bool isConst,Range range = Range.EightBit, bool isUnsigned = false) : PerenType(isConst, range, isUnsigned)
{
    public override bool CanAccept(PerenType type)
    {
        if (this.IsConst && !type.IsConst)
            return false;
        return type is CharType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }

    public override bool OpAccept(Tokens op)
    {
        return op.tokenType switch
        {
            TokenType.Addition
                or TokenType.Subtraction
                or TokenType.Division
                or TokenType.Multiplication
                or TokenType.Modulas
                or TokenType.LShift
                or TokenType.RShift
                or TokenType.And
                or TokenType.Or
                or TokenType.Xor
                or TokenType.Not => true,
            _ => false
        };
    }
}
