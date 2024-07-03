namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType(string typeName, Dictionary<string, LacusType> v) : LacusType(typeName, v)
{
    public override bool CanAccept(LacusType type)
    {
        return name == type.name;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}