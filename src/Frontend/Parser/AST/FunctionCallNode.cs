using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class FunctionCallNode(Tokens name, List<INode> paramValues) : StatementNode
{
    public List<INode> ParamValues { get; set; } = paramValues;
    public Tokens Name { get; set; } = name;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
