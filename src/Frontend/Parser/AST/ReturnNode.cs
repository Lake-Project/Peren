using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ReturnNode : StatementNode
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

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
