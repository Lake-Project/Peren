using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class VoidType(Range range = Range.none,bool isConst = false) : LacusType(isConst, range)
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