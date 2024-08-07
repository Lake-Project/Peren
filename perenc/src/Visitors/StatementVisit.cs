using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public abstract class StatementVisit
{
    public virtual void Visit(VaraibleDeclarationNode node)
    {
    }


    public virtual void Visit(VaraibleReferenceStatementNode node)
    {
    }

    public virtual void Visit(FunctionCallNode node)
    {
    }

    public virtual void Visit(FunctionNode node)
    {
    }

    public virtual void Visit(ReturnNode node)
    {
    }

    public virtual void Visit(ForLoopNode node)
    {
    }

    public virtual void Visit(WhileLoopNode node)
    {
    }

    public virtual void Visit(IfNode node)
    {
    }

    public virtual void Visit(ModuleNode moduleNode)
    {
    }

    public virtual void Visit(StructNode structNode)
    {
    }
}