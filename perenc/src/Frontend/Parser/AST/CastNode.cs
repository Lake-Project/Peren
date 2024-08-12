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

public class CastNode(ExpressionNode expr, Tokens type) : ExpressionNode
{
    public ExpressionNode Expr { get; set; } = expr;

    public Tokens type = type;

    public CastType inferredtype;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}