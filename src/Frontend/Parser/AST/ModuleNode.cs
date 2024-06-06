using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ModuleNode : StatementNode
{
    public List<StatementNode> StatementNodes = new();

    public Tokens name;

    public override void Visit(StatementVisit visitor)
    {
        base.Visit(visitor);
    }
}
