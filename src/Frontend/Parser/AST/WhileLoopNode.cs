using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class WhileLoopNode : INode
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
}
