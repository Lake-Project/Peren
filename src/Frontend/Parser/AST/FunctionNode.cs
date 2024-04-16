using Lexxer;
using LLVMSharp.Interop;

public class FunctionNode : INode
{
    public List<INode?> statements;
    public bool isExtern;
    LLVMTypeRef retType;
    public string name;

    public FunctionNode(string name, LLVMTypeRef retType, List<INode?> statements)
    {
        this.name = name;
        this.retType = retType;
        this.statements = statements;
    }

    public FunctionNode(Tokens name, List<INode?> statements)
    {
        this.name = name.buffer;
        this.statements = statements;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // return visitor.visit(this, builder, module);
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(retType, new LLVMTypeRef[0] { }, false);
        LLVMValueRef function = module.AddFunction(name, funcType);
        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        context.AllocateScope();
        context.CurrentRetType = retType;
        builder.PositionAtEnd(entry);
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
