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
        // Dictionary<LLVMTypeRef, IVisitor> visitors =
        // 	new()
        // 	{
        // 		[LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
        // 		[LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
        // 		[LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
        // 		[LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
        // 		[LLVMTypeRef.Float] = new FloatExprVisitor(),
        // 	};
        scope.Setret();
        // if (!visitors.TryGetValue(scope.CurrentRetType, out IVisitor? value))
        // {
        // 	return builder.BuildRetVoid();
        // }
        if (scope.CurrentRetType == LLVMTypeRef.Void)
            return builder.BuildRetVoid();
        if (expression == null)
            throw new Exception("must return");

        LLVMValueRef returnExpresion = scope.HandleTypes(
            scope.CurrentRetType,
            builder,
            module,
            expression
        );
        return builder.BuildRet(returnExpresion);
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
