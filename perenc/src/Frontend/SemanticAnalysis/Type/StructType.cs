using Lexxer;

namespace LacusLLVM.Frontend.SemanticAnalysis;

public class StructType(string typeName, Dictionary<string, PerenType> v, bool isConst)
    : PerenType(typeName, v, isConst)
{
    public override bool CanAccept(PerenType type)
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