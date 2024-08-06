using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class IntegerNode(long n) : ExpressionNode
{
    public Range Range { get; set; }

    public IntegerNode(Tokens value) : this(long.Parse(value.buffer))
    {
    }

    public long Value { get; set; } = n;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
    public override string ToString()
    {
        return Value.ToString();
    }
}