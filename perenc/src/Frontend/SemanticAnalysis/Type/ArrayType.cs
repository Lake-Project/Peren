using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class ArrayType(PerenType type, bool isConst) : PerenType(type, isConst)
{
    public override bool CanAccept(PerenType type)
    {
        if (this.IsConst && !type.IsConst)
            return false;
        if (simplerType != null && type.simplerType != null)
            return simplerType.CanAccept(type.simplerType);
        return false;
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