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
            return builder.BuildStore(eq, b.valueRef);
        }
    }
}
