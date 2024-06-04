using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class StatementNode : INode
{
    public virtual T Visit<T>(ExpressionVisit<T> visit)
    {
        throw new NotImplementedException();
    }

    public virtual void Visit(StatementVisit visitor)
    {
        throw new NotImplementedException();
    }
}
