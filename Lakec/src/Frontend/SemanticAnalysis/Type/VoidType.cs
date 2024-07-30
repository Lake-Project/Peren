using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class VoidType(bool isConst = false) : LacusType(isConst)
{
    public override bool CanAccept(LacusType type)
    {
        return type is VoidType;
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