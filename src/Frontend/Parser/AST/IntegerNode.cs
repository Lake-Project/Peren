using Lexxer;
using LLVMSharp.Interop;

public class IntegerNode : INode
{


	private int num;


	public IntegerNode(Tokens num)
	{
		this.num = int.Parse(num.buffer);

	}
	public IntegerNode(int num)
	{
		this.num = num;

	}


	public unsafe LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return Visit(builder, module);
	}

	public unsafe LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return LLVM.ConstInt(LLVM.Int32Type(), (ulong)num, 1);

	}
}