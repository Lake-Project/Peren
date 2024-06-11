using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class CastNode(INode expr, Tokens type) : INode
{
    public INode Expr { get; set; } = expr;

    public Tokens type = type;
    
    public Tokens inferredtype;

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}