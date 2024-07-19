using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayRefNode(Tokens Name, INode elem) : VaraibleReferenceNode(Name)
{
    public INode Elem { get; set; } = elem; //l
}