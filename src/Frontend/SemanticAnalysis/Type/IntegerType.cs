namespace LacusLLVM.Frontend.SemanticAnalysis;

public class IntegerType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return (type is IntegerType) || (type is CharType);
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
