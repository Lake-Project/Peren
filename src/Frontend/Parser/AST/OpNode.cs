using Lexxer;
using LLVMSharp.Interop;

public class OpNode : INode
{
	public INode? Left { get; set; }
	public INode? Right { get; set; }
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
		return Visit(builder, module);
	}

	public unsafe LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		if (Left == null && Right != null)
		{
			return Right.Accept(builder, module);
		}
		else if (Right == null && Left != null)
		{
			return Left.Accept(builder, module);
		}
		else if (Right == null && Left == null)
		{
			return LLVM.ConstInt(LLVM.Int32Type(), (ulong)0, 1);
		}
		else
		{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			return Op.tokenType switch
			{
				TokenType.ADD => builder.BuildAdd(Left.Accept(builder, module), Right.Accept(builder, module), "addtmp"),
				TokenType.SUBTRACT => builder.BuildSub(Left.Accept(builder, module), Right.Accept(builder, module), "subtmp"),
				TokenType.DIVIDE => builder.BuildSDiv(Left.Accept(builder, module), Right.Accept(builder, module), "divtmp"),
				TokenType.MULTIPLY => builder.BuildMul(Left.Accept(builder, module), Right.Accept(builder, module), "multmp"),
				TokenType.MODULAS => builder.BuildSRem(Left.Accept(builder, module), Right.Accept(builder, module), "modtmp"),
				_ => builder.BuildAdd(Left.Accept(builder, module), Right.Accept(builder, module), "addtmp")
			};
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		}

	}
}