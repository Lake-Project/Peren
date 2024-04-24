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
        Function fun = context.GetFunction(this.Name);
        LLVMValueRef[] values = new LLVMValueRef[fun.f.Parameters.Count];
        LLVMTypeRef[] differTypes = fun.f.paramTypes;

        for (int i = 0; i < values.Length; i++)
        {
            if (
                differTypes[i] == LLVMTypeRef.Int32
                || differTypes[i] == LLVMTypeRef.Int8
                || differTypes[i] == LLVMTypeRef.Int1
            )
            {
                values[i] = ParamValues[i].CodeGen(
                    new IntegerExpressionVisitor(),
                    builder,
                    module,
                    context
                );
            }
            else if (differTypes[i] == LLVMTypeRef.Float)
            {
                values[i] = ParamValues[i].CodeGen(
                    new FloatExprVisitor(),
                    builder,
                    module,
                    context
                );
            }
        }
        return visitor.Visit(this, builder, module, context);
    }
}
