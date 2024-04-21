using LLVMSharp.Interop;

public class VaraibleDeclarationNode : INode
{
    public INode? ExpressionNode;
    public LLVMTypeRef typeRef;
    public string name;
    public bool isExtern;

    public VaraibleDeclarationNode(
        LLVMTypeRef type,
        string name,
        INode? ExpressionNode,
        bool isExtern
    )
    {
        this.ExpressionNode = ExpressionNode;
        this.typeRef = type;
        this.name = name;
        this.isExtern = isExtern;
    }

    public void AddToScope(LLVMBuilderRef builder, Context context, LLVMValueRef value)
    {
        // LLVMValueRef b = builder.BuildAlloca(typeRef, name);
        context.AddNewVar(typeRef, name, builder.BuildAlloca(typeRef, name));
        Var l = context.GetVar(name);
        builder.BuildStore(value, l.valueRef);
    }

    // public VaraibleDeclarationNode()
    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        LLVMValueRef b;

        if (context.ScopeSize() == 0)
        {
            b = module.AddGlobal(typeRef, name);
            // b = builder.Build(typeRef, name);
            context.AddNewVar(typeRef, name, b);
            if (isExtern)
            {
                b.Linkage = LLVMLinkage.LLVMExternalLinkage;
            }

            if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
            {
                unsafe
                {
                    // LLVM.SetLinkage(b, LLVMLinkage.LLVMExternalLinkage);
                    LLVM.SetInitializer(
                        b,
                        ExpressionNode.CodeGen(
                            new IntegerExpressionVisitor(),
                            builder,
                            module,
                            context
                        )
                    ); // Initialize the global variable with value 42
                }
            }
            return b;
        }
        else
        {
            // Console.WriteLine(context.ScopeSize());
            b = builder.BuildAlloca(typeRef, name);
        }
        context.AddNewVar(typeRef, name, b);
        if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
        {
            return builder.BuildStore(
                ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, context),
                b
            );
        }
        if (typeRef == LLVMTypeRef.Float && ExpressionNode != null)
        {
            return builder.BuildStore(
                ExpressionNode.CodeGen(new FloatExprVisitor(), builder, module, context),
                b
            );
        }
        return b;
    }
}
