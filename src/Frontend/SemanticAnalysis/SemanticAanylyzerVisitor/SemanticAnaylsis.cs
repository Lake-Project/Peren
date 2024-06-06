using LacusLLVM.Frontend.Parser.AST;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnaylsis
{
    public void SemanticEntry(List<StatementNode> nodes)
    {
        SemanticVisitStatement s = new SemanticVisitStatement();
        nodes.ForEach(n => n.Visit(s));
    }
}
