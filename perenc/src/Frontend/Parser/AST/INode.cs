using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public interface INode
{
    public void Visit(Visitor v);
    // public T a<T>(ExpressionVisit<T> visit);
}