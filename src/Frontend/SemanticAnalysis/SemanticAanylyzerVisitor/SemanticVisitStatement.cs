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
    public SemanticContext<SemanticVar> _Context { get; set; }

    public SemanticVisitStatement()
    {
        _Context = new();
    }

    public override void Visit(VaraibleDeclarationNode node)
    {
        _Context.AddValue(
            node.name,
            new SemanticVar(new LacusType(TypeEnum.INTEGER), _Context.GetSize())
        );
        node.ExpressionNode.Visit(new SemanticVisitExpr(_Context, new LacusType(TypeEnum.INTEGER)));
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = _Context.GetValue(node.name);
        LacusType l = node.expression.Visit(new SemanticVisitExpr(_Context, v.VarType));
        node.ScopeLocation = v.ScopeLocation;
    }

    public override void Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionNode node)
    {
        _Context.AllocateScope();
        node.Parameters.ForEach(n => n.Visit(this));
        node.statements.ForEach(n => n.Visit(this));
        _Context.DeallocateScope();
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
