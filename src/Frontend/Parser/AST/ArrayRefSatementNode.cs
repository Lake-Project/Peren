using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayRefStatementNode(Tokens name,INode expr, INode element) : VaraibleReferenceStatementNode(name, expr)
{
    public INode Element { get; set; } = element;

}