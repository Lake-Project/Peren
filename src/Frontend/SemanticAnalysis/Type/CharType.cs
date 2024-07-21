using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class CharType(bool isConst) : LacusType(isConst)
{
    public override bool CanAccept(LacusType type)
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
            TokenType.ADDITION
                or TokenType.SUBTRACTION
                or TokenType.DIVISION
                or TokenType.MULTIPLICATION
                or TokenType.MODULAS
                or TokenType.L_SHIFT
                or TokenType.R_SHIFT
                or TokenType.AND
                or TokenType.OR
                or TokenType.NOT => true,
            _ => false
        };
    }
}
