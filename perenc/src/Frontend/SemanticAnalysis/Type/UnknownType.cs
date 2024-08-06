using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class UnknownType(bool isConst, Range range) : LacusType(isConst, range)
{
    public override bool CanAccept(LacusType type)
    {
        return true;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }

    public override bool OpAccept(Tokens op)
    {
        return false;

    }
}