using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class IntegerNode(int n) : INode
{
    public IntegerNode(Tokens value) : this(int.Parse(value.buffer))
    {
    }

    public int Value { get; set; } = n;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override string ToString()
    {
        return Value.ToString();
    }
}