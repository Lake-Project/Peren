namespace LacusLLVM.Frontend.SemanticAnalysis;

public class BoolType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return type is BoolType;
    }
}
