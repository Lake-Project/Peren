using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class FloatNode(float n) : ExpressionNode
{
    public float Value { get; set; } = n;

    public FloatNode(Tokens n)
        : this(float.Parse(n.buffer)) { }

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
