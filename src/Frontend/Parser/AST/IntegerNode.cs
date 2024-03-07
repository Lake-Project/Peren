using Lexxer;
using LLVMSharp.Interop;

public class IntegerNode : INode
{
	public INode? left { get; set; }
	public INode? right { get; set; }
	public Tokens op { get; set; }

	private Tokens num;
	public IntegerNode(Tokens num)
	{
		this.num = num;

	}
	public unsafe LLVMValueRef CodeGen(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		// throw new NotImplementedException();
		return LLVM.ConstInt(LLVM.Int32Type(), (ulong)int.Parse(num.buffer), 1);
	}
}