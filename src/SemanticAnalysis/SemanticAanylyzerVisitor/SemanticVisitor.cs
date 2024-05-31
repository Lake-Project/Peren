namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitor : SemanticVisit
{
    private Context _context;

    public SemanticVisitor(Context context)
    {
        this._context = context;
    }

    public override LacusType SemanticAccept(OpNode node)
    {
        LacusType LType = node.right.VisitSemanticAnaylsis(this);
        LacusType RType = node.left.VisitSemanticAnaylsis(this);
        if (LType.Type != RType.Type || LType.Type == TypeEnum.VOID || RType.Type == TypeEnum.VOID)
            throw new Exception("type error");
        return LType;
    }

    public override LacusType SemanticAccept(IntegerNode node)
    {
        return new LacusType(TypeEnum.INTEGER);
    }

    public override LacusType SemanticAccept(FloatNode node)
    {
        return new LacusType(TypeEnum.FLOAT);
    }

    public override LacusType SemanticAccept(VaraibleReferenceNode node)
    {
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(VaraibleReferenceStatementNode node)
    {
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(FunctionNode node)
    {
        _context.AllocateScope();
        node.statements.ForEach(n => n.VisitSemanticAnaylsis(this));
        _context.DeallocateScope();
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(VaraibleDeclarationNode node)
    {
        if (node.ExpressionNode != null)
            node.ExpressionNode.VisitSemanticAnaylsis(this);
        return new LacusType(TypeEnum.VOID);
    }
}
