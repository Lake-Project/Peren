using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public interface INode
{
    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    );

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor);
}
