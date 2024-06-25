namespace LacusLLVM.Frontend.SemanticAnalysis;

public class VoidType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return type is VoidType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
