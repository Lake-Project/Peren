using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class FunctionNode(
    AttributesTuple attributesTuple,
    Tokens name,
    List<VaraibleDeclarationNode> parameters,
    TypeNode retType,
    List<StatementNode> statements
) : StatementNode
{
    public List<StatementNode> Statements { get; set; } = statements;
    // public bool IsExtern { get; set; } = isExtern;
    public AttributesTuple AttributesTuple { get; set; }= attributesTuple;
    public List<VaraibleDeclarationNode> Parameters { get; set; } = parameters;
    public TypeNode RetType { get; set; } = retType;

    public Tokens Name { get; set; } = name;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);

    // public override string ToString()
    // {
    //     return $"{Name} : {statements.Count} : {RetType}";
    // }
}