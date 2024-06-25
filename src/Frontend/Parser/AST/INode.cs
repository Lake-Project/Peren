using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public interface INode
{
    public T Visit<T>(ExpressionVisit<T> visit);
}
