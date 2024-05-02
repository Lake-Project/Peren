using LLVMSharp.Interop;
using Lexxer;

public class FloatNode : INode
{
    public float n;

    public FloatNode(float n)
    {
        this.n = n;
    }

    public FloatNode(Tokens n)
    {
        this.n = float.Parse(n.buffer);
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
		return visitor.Visit(this, builder, module, context);
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
