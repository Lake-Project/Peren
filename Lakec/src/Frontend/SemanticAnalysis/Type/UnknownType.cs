using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class UnknownType(bool isConst) : LacusType(isConst)
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