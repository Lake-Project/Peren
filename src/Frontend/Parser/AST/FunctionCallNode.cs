using LLVMSharp;
using LLVMSharp.Interop;

public class FunctionCallNode : INode
{
    private List<INode?> ParamValues;
    public string Name;
    public LLVMValueRef[] Values;

    public FunctionCallNode(string name, List<INode?> ParamValues)
    {
        this.Name = name;
        this.ParamValues = ParamValues;
        Values = new LLVMValueRef[0];
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // Dictionary<LLVMTypeRef, IVisitor> visitors =
        //     new()
        //     {
        //         [LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
        //         [LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
        //         [LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
        //         [LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
        //         [LLVMTypeRef.Float] = new FloatExprVisitor(),
        //     };
        Function fun = context.GetFunction(this.Name);
        LLVMValueRef[] values = new LLVMValueRef[fun.f.Parameters.Count];
        LLVMTypeRef[] differTypes = fun.f.paramTypes;

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = context.HandleTypes(differTypes[i], builder, module, ParamValues[i]);
        }
        return visitor.Visit(this, builder, module, context);
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
