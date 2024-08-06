using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayRefStatementNode(Tokens name,ExpressionNode expr, ExpressionNode element) : VaraibleReferenceStatementNode(name, expr)
{
    public ExpressionNode Element { get; set; } = element;

}