using LLVMSharp.Interop;

public class ReturnStatement : INode
{
	public INode expression;

	public ReturnStatement(INode expression)
	{
		this.expression = expression;
	}
	public LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return Visit(builder, module);
	}

	public LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return builder.BuildRet(expression.Accept(builder, module));
	}
}