using LLVMSharp.Interop;

public class CastNode : INode
{
	public INode expr;
	public LLVMTypeRef castType;

	public CastNode(INode expr, LLVMTypeRef castType)
	{
		this.expr = expr;
		this.castType = castType;
	}

	public LLVMValueRef CodeGen(
		IVisitor visitor,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
	{
		
		throw new NotImplementedException();
	}

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
