using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ForLoopNode : INode
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

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
