using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class OpNode(ExpressionNode left, ExpressionNode right, Tokens tokens) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = left;
    public ExpressionNode Right { get; set; } = right;
    public Tokens Token { get; set; } = tokens;
    public bool FloatExpr { get; set; } = false;
    public bool IsUnsignedExpr { get; set; } = false;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override string ToString()
    {
        if (Left == null && Right != null)
        {
            return Right.ToString() + " " + Token.ToString();
        }
        else if (Right == null && Left != null)
        {
            return Left.ToString() + " " + Token.ToString();
        }
        else if (Right != null && Left != null)
        {
            return Left.ToString() + " " + Token.ToString() + " " + Right.ToString();
        }
        else
        {
            return "NULL";
        }
    }
}