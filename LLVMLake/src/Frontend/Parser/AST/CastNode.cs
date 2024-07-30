using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class CastNode(INode expr, Tokens type) : INode
{
    public INode Expr { get; set; } = expr;

    public Tokens type = type;

    public Tokens inferredtype;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
