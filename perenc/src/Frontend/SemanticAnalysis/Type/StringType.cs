using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StringType(bool isConst, Range range) : LacusType(isConst, range)
{
    public override bool CanAccept(LacusType type)
    {
        throw new NotImplementedException();
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