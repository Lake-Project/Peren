using Lexxer;
using LLVMSharp.Interop;
public interface INode
{


	public unsafe LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module);
	public unsafe LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module);
}