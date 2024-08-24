using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class VoidType(Range range = Range.None,bool isConst = false) : PerenType(isConst, range)
{
    public override bool CanAccept(PerenType type)
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