using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.Frontend.Parser.AST;

public class TupleNode(List<INode> expr) : INode
{
    public T Visit<T>(ExpressionVisit<T> visit)
    {
        throw new NotImplementedException();
    }
}