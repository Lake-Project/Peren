using LacusLLVM.Frontend.Parser.AST;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticAnaylsis
{
    public void SemanticEntry(List<INode> nodes)
    {
        nodes.ForEach(n => n.VisitSemanticAnaylsis(new SemanticVisitor(new Context())));
    }
}
