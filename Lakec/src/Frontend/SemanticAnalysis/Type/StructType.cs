using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType(string typeName, Dictionary<string, LacusType> v, bool isConst)
    : LacusType(typeName, v, isConst)
{
    public override bool CanAccept(LacusType type)
    {
        return name == type.name;
    }

    public override int size()
    {
        throw new NotImplementedException();
    }

    public override bool OpAccept(Tokens op)
    {
        return false;

    }
}