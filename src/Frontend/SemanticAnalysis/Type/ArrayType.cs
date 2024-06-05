namespace LacusLLVM.Frontend.SemanticAnalysis;

public class ArrayType : LacusType
{
    public ArrayType(LacusType type)
        : base(type) { }

    public override bool CanAccept(LacusType type)
    {
        if (simplerType != null && type.simplerType != null)
            return simplerType.CanAccept(type.simplerType);
        return false;
    }
}
