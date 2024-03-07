using Lexxer;
using LLVMSharp.Interop;

public class OpNode : INode
{
	public INode? left { get; set; }
	public INode? right { get; set; }
	public Tokens op { get; set; }
	public OpNode(INode left, INode right, Tokens op)
	{
		this.left = left;
		this.right = right;
		this.op = op;
	}
	public LLVMValueRef CodeGen(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		throw new NotImplementedException();
	}
}