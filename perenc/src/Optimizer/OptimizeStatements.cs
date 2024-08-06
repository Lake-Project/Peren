using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor.Optimizer;

public class OptimizeStatements : StatementVisit
{
    public override void Visit(VaraibleDeclarationNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ReturnNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ForLoopNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(WhileLoopNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(IfNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(StructNode node)
    {
        throw new NotImplementedException();
    }
}