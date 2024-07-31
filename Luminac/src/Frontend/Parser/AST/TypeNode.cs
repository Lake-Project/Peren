using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class TypeNode(Tokens name, AttributesTuple tuple)
{
//     public bool IsUnsigned { get; set; } = isUnsigned;
//     public bool IsExtern { get; set; } = isExtern;
//     public bool IsConst { get; set; } = isConst;?
    public AttributesTuple tuple { get; set; } = tuple;
    public Tokens Name { get; set; } = name;
}