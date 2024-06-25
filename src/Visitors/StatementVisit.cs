using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public abstract class StatementVisit
{
    public abstract void Visit(VaraibleDeclarationNode node);
    public abstract void Visit(VaraibleReferenceStatementNode node);
    public abstract void Visit(FunctionCallNode node);
    public abstract void Visit(FunctionNode node);
    public abstract void Visit(ReturnNode node);
    public abstract void Visit(CastNode node);
    public abstract void Visit(ForLoopNode node);
    public abstract void Visit(WhileLoopNode node);
    public abstract void Visit(IfNode node);
    public abstract void Visit(StructNode node);

}
