using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class BoolNode(bool value) : INode
{
    public bool Value { get; set; } = value;
    private INode _nodeImplementation;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
