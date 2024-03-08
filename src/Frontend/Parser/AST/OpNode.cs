using Lexxer;
using LLVMSharp.Interop;

public class OpNode : INode
{
	public INode Left { get; set; }
	public INode Right { get; set; }
	public Tokens Op { get; set; }
	public OpNode(INode left, INode right, Tokens op)
	{
		this.Left = left;
		this.Right = right;
		this.Op = op;
	}
	public OpNode(INode left, INode right)
	{
		this.Left = left;
		this.Right = right;
	}
	public OpNode(INode left)
	{
		this.Left = left;
		Right = new IntegerNode(0);
	}


	public unsafe LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return this.Visit(builder, module);
	}

	public unsafe LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return builder.BuildAdd(Left.Accept(builder, module),
					Right.Accept(builder, module), "addtmp");
	}
}