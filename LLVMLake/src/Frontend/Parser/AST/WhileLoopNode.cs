using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class WhileLoopNode(INode expr, List<StatementNode> statementNodes) : StatementNode
{
    public INode Expression { get; set; } = expr;

    public List<StatementNode> StatementNodes { get; set; } = statementNodes;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
