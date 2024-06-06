namespace LacusLLVM.Frontend.SemanticAnalysis;

public class VoidType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return type is VoidType;
    }
}
