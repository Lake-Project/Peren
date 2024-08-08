using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ModuleNode(Tokens name, List<Tokens> imports) : StatementNode
{
    // public List<TopLevelStatement> StatementNodes = new();

    public List<FunctionNode> FunctionNodes = new();
    public List<StructNode> StructNodes = new();
    public List<VaraibleDeclarationNode> VaraibleDeclarationNodes = new();
    public Tokens Name { get; set; } = name;
    public List<Tokens> Imports { get; set; } = imports;


    public override void Visit(StatementVisit v) => v.Visit(this);
}