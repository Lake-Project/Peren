using LLVMSharp.Interop;
using Lexxer;
public class IntegerNode : INode
{
	public int n;
	public IntegerNode(int n)
	{
		this.n = n;
	}
	public IntegerNode(Tokens n)
	{
		this.n = int.Parse(n.buffer);
	}
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{

		return visitor.Visit(this, builder, module, ref scope);

		// throw new NotImplementedException();
	}
}