using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class CastNode : INode
{
    public INode expr;
    public LLVMTypeRef castType;

    public CastNode(INode expr, LLVMTypeRef castType)
    {
        this.expr = expr;
        this.castType = castType;
    }

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
