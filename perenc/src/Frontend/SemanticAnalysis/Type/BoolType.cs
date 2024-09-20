using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class BoolType(bool isConst,Range range = Range.OneBit) : PerenType(isConst, range)
{
    public override bool CanAccept(PerenType type)
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
            TokenType.And
                or TokenType.Or
                or TokenType.Gt
                or TokenType.Lt
                or TokenType.BoolEq
                or TokenType.Gte
                or TokenType.Not
                or TokenType.Lte
                or TokenType.NotEquals
                => true,
            _ => false
        };
    }
}