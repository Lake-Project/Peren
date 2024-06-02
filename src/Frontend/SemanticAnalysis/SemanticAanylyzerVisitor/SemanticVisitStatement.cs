using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public LacusType VarType { get; set; }
    public int ScopeLocation { get; set; }

    public SemanticVar(LacusType type, int scopeLocation)
    {
        VarType = type;
        ScopeLocation = scopeLocation;
    }
}

public class SemanticVisitStatement : StatementVisit
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

    public override void Visit(CastNode node)
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
}
