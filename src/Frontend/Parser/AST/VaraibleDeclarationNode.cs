using System.Linq.Expressions;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleDeclarationNode : INode
{
    public INode? ExpressionNode;
    public LLVMTypeRef typeRef;
    public Tokens name;
    public bool isExtern;
    public bool isStruct;

    public VaraibleDeclarationNode(
        LLVMTypeRef type,
        Tokens name,
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
        context.AddNewVar(typeRef, name, builder.BuildAlloca(typeRef, name.buffer));
        Var l = context.GetVar(name);
        builder.BuildStore(value, l.valueRef);
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        LLVMValueRef b;
        if (context.ScopeSize() == 0)
            b = module.AddGlobal(typeRef, name.buffer);
        else
            b = builder.BuildAlloca(typeRef, name.buffer);
        if (isExtern)
        {
            b.Linkage = LLVMLinkage.LLVMExternalLinkage;
            return b;
        }
        if (ExpressionNode == null)
        {
            return b;
        }
        LLVMValueRef eq = context.HandleTypes(typeRef, builder, module, ExpressionNode);
        context.AddNewVar(typeRef, name, b);
        if (context.ScopeSize() == 0)
        {
            unsafe
            {
                LLVM.SetInitializer(b, eq);
                return b;
            }
        }
        else
        {
            return builder.BuildStore(eq, b);
        }
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
