using LLVMSharp.Interop;

public interface IVisitor
{
	public LLVMValueRef visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module);
	public LLVMValueRef visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module);

	public LLVMValueRef visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module);



}