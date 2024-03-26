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
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{
		// return solve.Solve(this, builder, module);
		return visitor.visit(this, builder, module, ref scope);
	}
}