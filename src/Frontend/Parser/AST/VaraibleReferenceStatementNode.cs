using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : StatementNode
{
    public INode expression;
    public Tokens name;
    public int ScopeLocation { get; set; }

    public VaraibleReferenceStatementNode(Tokens name, INode expresion)
    {
        this.name = name;
        this.expression = expresion;
    }

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
