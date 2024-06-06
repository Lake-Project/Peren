using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.Frontend.Parser.AST;

public class ArrayNode : StatementNode
{
    public INode size;
    public Tokens type;
    public ArrayNode NextDimension;

    public override void Visit(StatementVisit visitor)
    {
        throw new NotImplementedException();
    }
}
