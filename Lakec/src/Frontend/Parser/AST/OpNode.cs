using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class OpNode(INode left, INode right, Tokens tokens) : INode
{
    public INode Left { get; set; } = left;
    public INode Right { get; set; } = right;
    public Tokens Token { get; set; } = tokens;
    public bool FloatExpr { get; set; } = false;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

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