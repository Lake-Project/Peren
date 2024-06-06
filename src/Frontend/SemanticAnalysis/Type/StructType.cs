namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType : LacusType
{
    public StructType(string typeName, List<LacusType> v)
        : base(typeName, v) { }

    public override bool CanAccept(LacusType type)
    {
        if (type.name == this.name) { }
        throw new NotImplementedException();
    }
}
