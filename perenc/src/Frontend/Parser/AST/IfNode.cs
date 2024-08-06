using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class IfNode(ExpressionNode expression, ElseNode elseNode, List<StatementNode> statementNodes)
    : StatementNode
{
    public ExpressionNode Expression { get; set; } = expression;

    public ElseNode ElseNode { get; set; } = elseNode;
    public List<StatementNode> StatementNodes { get; set; } = statementNodes;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
    public override string ToString()
    {
        return Expression.ToString();
    }
}
