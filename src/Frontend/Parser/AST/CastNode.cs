using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class CastNode : StatementNode
{
    public INode expr;

    public CastNode(INode expr)
    {
        this.expr = expr;
    }

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
