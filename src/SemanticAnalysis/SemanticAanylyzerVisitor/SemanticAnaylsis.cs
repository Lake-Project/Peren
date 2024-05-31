using LacusLLVM.Frontend.Parser.AST;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnaylsis
{
    public void SemanticEntry(List<StatementNode> nodes)
    {
        nodes.ForEach(n =>
            n.VisitSemanticAnaylsis(new SemanticVisitor(new SemanticContext<SemanticVar>()))
        );
    }
}
