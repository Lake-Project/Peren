using System.Linq.Expressions;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceNode : INode
{
    public Tokens name;

    public VaraibleReferenceNode(Tokens varName)
    {
        name = varName;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // throw new NotImplementedException();
        return visitor.Visit(this, builder, module, context);
    }

    public override string ToString()
    {
        return name.ToString();
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
