using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class TypeNode(Tokens name, bool isUnsigned, bool isConst, bool isExtern)
{
    public bool IsUnsigned { get; set; } = isUnsigned;
    public bool IsExtern { get; set; } = isExtern;
    public bool IsConst { get; set; } = isConst;
    public Tokens Name { get; set; } = name;
}