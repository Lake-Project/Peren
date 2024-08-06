using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;


public class ReturnNode(ExpressionNode? expression) : StatementNode
{
    public ExpressionNode? Expression { get; set; } = expression;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
