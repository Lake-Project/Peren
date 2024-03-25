using LLVMSharp.Interop;

public class IntegerNode : INode
{
	public int n;
	public IntegerNode(int n)
	{
		this.n = n;
	}

	public LLVMValueRef CodeGen(IVisitor visitor,LLVMBuilderRef builder, LLVMModuleRef module)
	{
		
			return visitor.visit(this,builder, module);

		// throw new NotImplementedException();
	}
}