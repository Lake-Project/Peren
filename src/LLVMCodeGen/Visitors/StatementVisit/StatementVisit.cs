using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public abstract class StatementVisit
{
    public abstract void Visit(VaraibleDeclarationNode node);
    public abstract void Visit(VaraibleReferenceStatementNode node);
    public abstract void Visit(FunctionCallNode node);
    public abstract void Visit(FunctionNode node);
}
