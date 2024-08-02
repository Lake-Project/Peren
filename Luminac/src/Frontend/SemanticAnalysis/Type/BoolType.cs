using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class BoolType(bool isConst,Range range = Range.one_bit) : LacusType(isConst, range)
{
    public override bool CanAccept(LacusType type)
    {
        if (this.IsConst && !type.IsConst)
            return false;
        return type is BoolType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }

    public override bool OpAccept(Tokens op)
    {
        return op.tokenType switch
        {
            TokenType.AND
                or TokenType.OR
                or TokenType.GT
                or TokenType.LT
                or TokenType.BOOL_EQ
                or TokenType.GTE
                or TokenType.NOT
                or TokenType.LTE
                or TokenType.NOT_EQUALS
                => true,
            _ => false
        };
    }
}