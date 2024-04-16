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
        scope.Setret();
        if (scope.CurrentRetType == LLVMTypeRef.Int32)
        {
            if (expression == null)
            {
                throw new Exception("needs valye");
            }
            return builder.BuildRet(
                expression.CodeGen(new IntegerExpressionVisitor(), builder, module, scope)
            );
        }
        else
        {
            return builder.BuildRetVoid();
        }
    }
}
