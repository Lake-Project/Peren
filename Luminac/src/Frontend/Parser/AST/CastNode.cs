using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;


public enum CastType
{
    INT,
    FLOAT,
    TRUNCATE,
    SEXT
}
public class CastNode(INode expr, Tokens type) : INode
{
    public INode Expr { get; set; } = expr;

    public Tokens type = type;

    public CastType inferredtype;

    public bool Truncate { get; set; }

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}