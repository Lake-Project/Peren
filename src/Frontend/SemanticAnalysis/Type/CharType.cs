namespace LacusLLVM.Frontend.SemanticAnalysis;

public class CharType : LacusType
{
    public override bool CanAccept(LacusType type)
    {
        return type is CharType;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
