namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType : LacusType
{
    public StructType(string typeName, List<LacusType> v)
        : base(typeName, v) { }

    public override bool CanAccept(LacusType type)
    {
        throw new NotImplementedException();
    }
}
