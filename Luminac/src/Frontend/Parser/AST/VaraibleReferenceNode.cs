using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class VaraibleReferenceNode(Tokens varName) : ExpressionNode
{
    public int ScopeLocation;
    public Tokens Name { get; set; } = varName;

    public override T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);

    public override string ToString()
    {
        return Name.ToString();
    }
}
