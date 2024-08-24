using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class UnknownType(bool isConst, Range range) : PerenType(isConst, range)
{
    public override bool CanAccept(PerenType type)
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