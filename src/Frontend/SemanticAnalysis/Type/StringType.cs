using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StringType(bool isConst) : LacusType(isConst)
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