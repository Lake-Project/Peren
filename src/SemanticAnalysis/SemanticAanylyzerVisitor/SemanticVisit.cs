namespace LacusLLVM.SemanticAanylyzerVisitor;

public abstract class SemanticVisit
{
    public abstract LacusType SemanticAccept(OpNode node);
    public abstract LacusType SemanticAccept(IntegerNode node);
    public abstract LacusType SemanticAccept(FloatNode node);
    public abstract LacusType SemanticAccept(VaraibleReferenceNode node);
    public abstract LacusType SemanticAccept(VaraibleReferenceStatementNode node);

    public abstract LacusType SemanticAccept(FunctionNode node);
    public abstract LacusType SemanticAccept(VaraibleDeclarationNode node);
}
