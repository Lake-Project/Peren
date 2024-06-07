using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class CastNode(INode expr) : StatementNode
{
    public INode Expr { get; set; } = expr;

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
