using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayNode(
    Tokens type,
    Tokens name,
    ExpressionNode size,
    AttributesTuple attributesTuple)
    : VaraibleDeclarationNode(type, name, attributesTuple)
{
    public ExpressionNode Size { get; set; } = size;

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}