using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ForLoopNode(VaraibleDeclarationNode iterator,
INode expr, StatementNode inc, List<StatementNode> statementNodes) : StatementNode
{
    public VaraibleDeclarationNode Iterator { get; set; } = iterator;
    public INode Expr { get; set; } = expr;
    public StatementNode Inc { get; set; } = inc;
    public List<StatementNode>  Statements { get; set; } = statementNodes;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
