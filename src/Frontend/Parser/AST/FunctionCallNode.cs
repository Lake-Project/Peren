using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class FunctionCallNode : StatementNode
{
    private List<INode> ParamValues;
    public Tokens Name;
    public LLVMValueRef[] Values;

    public FunctionCallNode(Tokens name, List<INode> ParamValues)
    {
        this.Name = name;
        this.ParamValues = ParamValues;
        Values = new LLVMValueRef[0];
    }

    // public LLVMValueRef CodeGen(
    //     IVisitor visitor,
    //     LLVMBuilderRef builder,
    //     LLVMModuleRef module,
    //     Context context
    // )
    // {
    //     // Dictionary<LLVMTypeRef, IVisitor> visitors =
    //     //     new()
    //     //     {
    //     //         [LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
    //     //         [LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
    //     //         [LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
    //     //         [LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
    //     //         [LLVMTypeRef.Float] = new FloatExprVisitor(),
    //     //     };
    //     Function fun = context.GetFunction(this.Name);
    //     LLVMValueRef[] values = new LLVMValueRef[fun.f.Parameters.Count];
    //     LLVMTypeRef[] differTypes = fun.f.paramTypes;
    //
    //     for (int i = 0; i < values.Length; i++)
    //     {
    //         values[i] = context.HandleTypes(differTypes[i], builder, module, ParamValues[i]);
    //     }
    //
    //     return visitor.Visit(this, builder, module, context);
    // }

    public override T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
