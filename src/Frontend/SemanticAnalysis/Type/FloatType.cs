namespace LacusLLVM.Frontend.SemanticAnalysis;

public class FloatType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return type is IntegerType || type is FloatType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
