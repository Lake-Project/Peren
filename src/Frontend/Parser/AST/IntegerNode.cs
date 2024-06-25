using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class IntegerNode(Tokens value) : INode
{
    public int Value { get; set; } = int.Parse(value.buffer);

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override string ToString()
    {
        return Value.ToString();
    }
}
