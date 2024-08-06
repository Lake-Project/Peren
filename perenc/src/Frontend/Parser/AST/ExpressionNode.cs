using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;


public abstract class ExpressionNode: StatementNode
{
    public abstract T Visit<T>(ExpressionVisit<T> visit);
    public void Visit(Visitor v)
    {
        throw new NotImplementedException();
    }
}