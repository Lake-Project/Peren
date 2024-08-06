using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ModuleNode : TopLevelStatement
{
    // public List<TopLevelStatement> StatementNodes = new();

    public List<FunctionNode> FunctionNodes = new();
    public List<StructNode> StructNodes = new();
    public List<VaraibleDeclarationNode> VaraibleDeclarationNodes = new();
    public Tokens name;


    public override void Visit(TopLevelVisitor v) => v.Visit(this);
}