using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class IfNode(
    INode expression,
    ElseNode elseNode,
    List<StatementNode> statementNodes) : StatementNode
{
    public INode Expression { get; set; } = expression;

    public ElseNode ElseNode { get; set; } = elseNode;
    public List<StatementNode> StatementNodes { get; set; } = statementNodes;

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}