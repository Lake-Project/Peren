using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class CastNode : INode
{
    public INode expr;
    public LLVMTypeRef castType;

    public LacusType From;
    public LacusType To;

    public CastNode(INode expr, LLVMTypeRef castType)
    {
        this.expr = expr;
        this.castType = castType;
    }

    public CastNode(INode expr, LacusType From, LacusType To)
    {
        this.From = From;
        this.To = To;
        this.expr = expr;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef Visit(ExpressionVisit visit)
    {
        throw new NotImplementedException();
    }

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        throw new NotImplementedException();
    }
}
