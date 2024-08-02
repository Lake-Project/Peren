using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class FloatType(bool isConst, Range range = Range.Float) : LacusType(isConst, range)
{
    public override bool CanAccept(LacusType type)
    {
        if (this.IsConst && !type.IsConst)
            return false;
        return type is IntegerType || type is FloatType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }

    public override bool OpAccept(Tokens op)
    {
        return op.tokenType switch
        {
            TokenType.ADDITION 
                or TokenType.SUBTRACTION 
                or TokenType.DIVISION 
                or TokenType.MULTIPLICATION => true,
            _ => false
        };
    }
}