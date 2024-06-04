using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class CastNode : StatementNode
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

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
