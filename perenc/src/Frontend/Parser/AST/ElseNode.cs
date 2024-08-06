namespace LacusLLVM.Frontend.Parser.AST;

public class ElseNode(List<StatementNode> statementNodes) : StatementNode
{
    public List<StatementNode> StatementNodes { get; set; } = statementNodes;
}