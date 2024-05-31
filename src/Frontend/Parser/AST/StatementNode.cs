using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class StatementNode : INode
{
    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
    }

    public virtual LLVMValueRef Visit(ExpressionVisit visit)
    {
        throw new NotImplementedException();
    }

    public virtual void Visit(StatementVisit visitor)
    {
        throw new NotImplementedException();
    }

    public virtual LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        throw new NotImplementedException();
    }
}
