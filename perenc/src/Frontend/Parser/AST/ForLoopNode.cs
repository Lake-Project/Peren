using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class ForLoopNode(VaraibleDeclarationNode iterator,
    ExpressionNode expr, StatementNode inc, List<StatementNode> statementNodes) : StatementNode
{
    public VaraibleDeclarationNode Iterator { get; set; } = iterator;
    public ExpressionNode Expr { get; set; } = expr;
    public StatementNode Inc { get; set; } = inc;
    public List<StatementNode>  Statements { get; set; } = statementNodes;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
