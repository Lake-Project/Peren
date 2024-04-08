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
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Scope scope)
	{

		return visitor.Visit(this, builder, module, scope);

		// throw new NotImplementedException();
	}
}