using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class WhileLoopNode : StatementNode
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

    public override void Visit(StatementVisit visitor)
    {
        throw new NotImplementedException();
    }

    public override LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        throw new NotImplementedException();
    }
}
