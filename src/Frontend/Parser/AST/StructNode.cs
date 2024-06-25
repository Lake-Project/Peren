using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class StructNode(
    Dictionary<string, (VaraibleDeclarationNode, int)> vars, Tokens name) : StatementNode
{
    public Dictionary<string, 
        (VaraibleDeclarationNode, int)> Vars { get; set; }= vars;
    public Tokens Name { get; set; } = name;

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
