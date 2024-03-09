using LLVMSharp.Interop;

public class VaraibleDecleartion : INode
{
	public INode? expression;
	public string name;

	public VaraibleDecleartion(string name, INode expression)
	{
		this.name = name;
		this.expression = expression;
	}
	public LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return Visit(builder, module);
	}

	public LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		LLVMValueRef b = builder.BuildAlloca(LLVMTypeRef.Int32, name);
		if (expression != null)
			return builder.BuildStore(expression.Accept(builder, module), b);
		else
			return b;
	}
}