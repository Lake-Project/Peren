using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class FunctionNode(
    Tokens name,
    List<VaraibleDeclarationNode> parameters,
    Tokens retType,
    List<StatementNode> statements,
    bool isExtern
) : StatementNode
{
    public List<StatementNode> Statements { get; set; } = statements;
    public bool IsExtern { get; set; } = isExtern;
    public List<VaraibleDeclarationNode> Parameters { get; set; } = parameters;
    public Tokens RetType { get; set; } = retType;

    public Tokens Name { get; set; } = name;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);

    // public override string ToString()
    // {
    //     return $"{Name} : {statements.Count} : {RetType}";
    // }
}
