using Lexxer;
using LLVMSharp.Interop;

public class FunctionNode : INode
{
    public List<INode?> statements;
    public bool isExtern;
    public List<VaraibleDeclarationNode> Parameters;
    LLVMTypeRef retType;
    public string name;

    public FunctionNode(
        string name,
        List<VaraibleDeclarationNode> Parameters,
        LLVMTypeRef retType,
        List<INode?> statements
    )
    {
        this.name = name;
        this.retType = retType;
        this.statements = statements;
        this.Parameters = Parameters;
    }

    public FunctionNode(Tokens name, List<INode?> statements)
    {
        this.name = name.buffer;
        this.statements = statements;
        this.Parameters = new List<VaraibleDeclarationNode>();
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        context.AllocateScope();
        LLVMTypeRef[] a = new LLVMTypeRef[Parameters.Count];
        for (int i = 0; i < Parameters.Count; i++)
        {
            a[i] = Parameters[i].typeRef;
        }
        // return visitor.visit(this, builder, module);
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(retType, a, false);

        LLVMValueRef function = module.AddFunction(name, funcType);

        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        context.CurrentRetType = retType;
        builder.PositionAtEnd(entry);
        for (int i = 0; i < Parameters.Count; i++)
        {
            Parameters[i].AddToScope(builder, context, function.GetParam((uint)i));
        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.

        for (int i = 0; i < statements.Count; i++)
        {
            statements[i].CodeGen(visitor, builder, module, context);
        }
        if (!context.GetRet())
        {
            if (retType == LLVMTypeRef.Void)
                builder.BuildRetVoid();
        }
        context.DeallocateScope();
        return function;
    }
}
