using Lexxer;
using LLVMSharp.Interop;

public class OpNode : INode
{
	public INode? left;
	public INode? right;
	public Tokens token;
	public OpNode(INode? left, INode? right)
	{
		this.left = left;
		this.right = right;
	}
	public OpNode(INode? left, INode? right, Tokens tokens)
	{
		this.left = left;
		this.right = right;
		this.token = tokens;
	}
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Scope scope)
	{
		// return solve.Solve(this, builder, module);
		return visitor.Visit(this, builder, module, scope);
	}
}