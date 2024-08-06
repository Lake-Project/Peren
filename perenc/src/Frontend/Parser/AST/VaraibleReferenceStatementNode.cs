using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class VaraibleReferenceStatementNode(Tokens name, ExpressionNode expresion) : StatementNode
{
    public ExpressionNode Expression { get; set; } = expresion;
    public Tokens Name { get; set; } = name;
    // public int ScopeLocation { get; set; }

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
