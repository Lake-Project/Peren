using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class BooleanExprNode(ExpressionNode left, ExpressionNode right, Tokens op) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = left;
    public ExpressionNode Right { get; set; } = right;
    public Tokens Op { get; set; } = op;
    public bool IsFloat { get; set; }
    public bool IsUnsigned { get; set; }

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override string ToString()
    {
        return $"{Left.ToString()}  {Op.ToString()}  {Right.ToString()}";
    }
}