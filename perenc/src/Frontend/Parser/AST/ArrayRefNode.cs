using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayRefNode(Tokens Name, ExpressionNode elem) : VaraibleReferenceNode(Name)
{
    public ExpressionNode Elem { get; set; } = elem; //l
}