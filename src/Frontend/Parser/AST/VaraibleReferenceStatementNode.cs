using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : INode
{
    public INode expression;
    public string name;

    public VaraibleReferenceStatementNode(string name, INode expresion)
    {
        this.name = name;
        this.expression = expresion;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        unsafe
        {
            Var b = context.GetVar(name);
            context.AddToTypeCheckerType(b.type);
            LLVMValueRef eq = context.HandleTypes(b.type, builder, module, expression);
            if (eq.IsConstant)
            {
                context.UpdateConst(name, eq);
                return b.constant;
            }
            else if (b.IsConstant)
                context.removeConst(name);
            return builder.BuildStore(eq, b.valueRef);
        }
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
