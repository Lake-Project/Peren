using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

public class VaraibleDeclarationNode(
    Tokens type,
    Tokens name,
    // INode? expressionNode,
    AttributesTuple attributesTuple
) : StatementNode
{
    public VaraibleDeclarationNode(Tokens type, Tokens name, AttributesTuple attributesTuple
        , INode expressionNode) : this(type, name, attributesTuple)
    {
        ExpressionNode = expressionNode;
    }

    public INode? ExpressionNode { get; set; }
    public Tokens Name { get; set; } = name;
    public Tokens Type { get; set; } = type;

    // public bool IsExtern { get; set; } = isExtern;

    public AttributesTuple AttributesTuple { get; set; } = attributesTuple;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}