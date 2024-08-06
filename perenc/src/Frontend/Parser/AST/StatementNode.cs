using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class StatementNode : INode
{
    public virtual void Visit(StatementVisit visitor)
    {
        throw new NotImplementedException();
    }

    public void Visit(Visitor v)
    {
        throw new NotImplementedException();
    }
}