namespace LacusLLVM.Frontend.SemanticAnalysis;

public class ArrayType(LacusType type) : LacusType(type)
{
    public override bool CanAccept(LacusType type)
    {
        if (simplerType != null && type.simplerType != null)
            return simplerType.CanAccept(type.simplerType);
        return false;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
