using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class FloatNode : INode
{
    public float n;

    public FloatNode(float n)
    {
        this.n = n;
    }

    public FloatNode(Tokens n)
    {
        this.n = float.Parse(n.buffer);
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        return visitor.Visit(this, builder, module, context);
    }

    public LLVMValueRef Visit(ExpressionVisit visit)
    {
        return visit.Visit(this);
    }

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        return visitor.SemanticAccept(this);
    }
}
