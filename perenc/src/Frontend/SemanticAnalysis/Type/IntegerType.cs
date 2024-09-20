using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class IntegerType(
    bool isConst,
    Range range = Range.ThirtyTwoBit,
    bool isUnsigned = false) : PerenType(isConst, range, isUnsigned)
{
    public override bool CanAccept(PerenType type)
    {
        if (this.IsConst && !type.IsConst)
        {
            Console.WriteLine("Types not const");
            return false;
        }

        return (type is IntegerType) ;
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
                or TokenType.LShift
                or TokenType.Modulas
                or TokenType.RShift
                or TokenType.And
                or TokenType.Or
                or TokenType.Xor
                or TokenType.Not => true,
            _ => false
        };
    }
}