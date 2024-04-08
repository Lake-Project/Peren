using LLVMSharp.Interop;

public class ReturnNode : INode
{
	public INode expression;
	public LLVMTypeRef type;

	public ReturnNode(LLVMTypeRef type, INode Expression)
	{
		this.expression = Expression;
		this.type = type;

	}
	// public 
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Scope scope)
	{
		if (scope.CurrentRetType == LLVMTypeRef.Int32)
		{
			return builder.BuildRet(expression.CodeGen(new IntegerExpressionVisitor(), builder, module, scope));
		}
		else
		{
			return builder.BuildRetVoid();
		}

	}
}