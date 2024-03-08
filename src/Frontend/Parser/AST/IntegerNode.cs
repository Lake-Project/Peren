using Lexxer;
using LLVMSharp.Interop;

public class IntegerNode : INode
{


	private Tokens num;
	private int n;


	public IntegerNode(Tokens num)
	{
		this.num = num;

	}
	public IntegerNode(int n)
	{
		this.n = n;

	}


	public unsafe LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return Visit(builder, module);
	}

	public unsafe LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{


		return LLVM.ConstInt(LLVM.Int32Type(), (ulong)n, 1);

	}
}