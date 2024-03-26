using LLVMSharp.Interop;

public class IntegerNode : INode
{
	public int n;
	public IntegerNode(int n)
	{
		this.n = n;
	}

	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{

		return visitor.Visit(this, builder, module, ref scope);

		// throw new NotImplementedException();
	}
}