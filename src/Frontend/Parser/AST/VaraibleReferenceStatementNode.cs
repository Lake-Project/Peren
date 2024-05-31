using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : INode
{
    public INode expression;
    public Tokens name;

    public VaraibleReferenceStatementNode(Tokens name, INode expresion)
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
            LLVMValueRef eq = context.HandleTypes(b.type, builder, module, expression);
            // if (eq.IsConstant)
            // {
            //     context.UpdateConst(name, eq);
            //     return b.constant;
            // }
            // else if (b.IsConstant)
            //     context.removeConst(name);
            return builder.BuildStore(eq, b.valueRef);
        }
    }

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        return visitor.SemanticAccept(this);
    }
}
