namespace LacusLLVM.Frontend.SemanticAnalysis;

public class UnknownType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return true;
    }
}