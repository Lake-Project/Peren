using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class BoolNode(bool value) : ExpressionNode
{
    public bool Value { get; set; } = value;
    private INode _nodeImplementation;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
