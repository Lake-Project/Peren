using LLVMSharp.Interop;

public class ReturnNode : INode
{
	public INode? expression;
	public LLVMTypeRef type;

	public ReturnNode(LLVMTypeRef type, INode Expression)
	{
		this.expression = Expression;
		this.type = type;
	}

	public ReturnNode(INode? Expression)
	{
		this.expression = Expression;
	}

	// public
	public LLVMValueRef CodeGen(
		IVisitor visitor,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context scope
	)
	{
		Dictionary<LLVMTypeRef, IVisitor> visitors =
			new()
			{
				[LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
				[LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
				[LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
				[LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
				[LLVMTypeRef.Float] = new FloatExprVisitor(),
			};
		scope.Setret();
		if (!visitors.TryGetValue(scope.CurrentRetType, out IVisitor? value))
		{
			return builder.BuildRetVoid();
		}
		if (expression == null)
			throw new Exception("must return");

		LLVMValueRef returnExpresion = expression.CodeGen(value, builder, module, scope);
		LLVMTypeRef type = scope.GetFromTypeChecker();
		if (type == scope.CurrentRetType)
			return builder.BuildRet(returnExpresion);
		
		if (scope.CurrentRetType.IntWidth <= type.IntWidth)
		{
			returnExpresion = builder.BuildTrunc(returnExpresion, scope.CurrentRetType, "RETURN TYPE");
		}
		else
		{
			returnExpresion = builder.BuildSExt(returnExpresion, scope.CurrentRetType, "RETURN TYPE");
		}
		return builder.BuildRet(returnExpresion);
	}
}
