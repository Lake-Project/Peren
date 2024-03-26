using LLVMSharp.Interop;

public interface IVisitor
{
	public LLVMValueRef visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope);
	public LLVMValueRef visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope);

	public LLVMValueRef visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope);



}