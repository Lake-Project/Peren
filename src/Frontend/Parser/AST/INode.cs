using LLVMSharp.Interop;

public interface INode
{
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope);
}