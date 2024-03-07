using Lexxer;
using LLVMSharp.Interop;
public interface INode
{
	public INode? left { get; set; }
	public INode? right { get; set; }
	public Tokens op { get; set; }

	public unsafe LLVMValueRef CodeGen(LLVMBuilderRef builder, LLVMModuleRef module);
}