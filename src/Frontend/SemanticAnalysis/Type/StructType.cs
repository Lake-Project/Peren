namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType(string typeName, List<LacusType> v) : LacusType(typeName, v)
{
    public override bool CanAccept(LacusType type)
    {
        if (type.name == this.name) { }
        throw new NotImplementedException();
    }

    public override int size()
    {
        throw new NotImplementedException();
    }
}
