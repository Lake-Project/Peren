using LLVMSharp.Interop;

public class CodeGenVisitor : IVisitor
{

	public LLVMValueRef visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		throw new NotImplementedException();
	}

	public LLVMValueRef visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		throw new NotImplementedException();
	}

	public LLVMValueRef visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		throw new NotImplementedException();
	}
}