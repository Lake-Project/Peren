using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleDeclarationNode(Tokens type, Tokens name, INode expressionNode, bool isExtern)
    : StatementNode
{
    public INode ExpressionNode { get; set; } = expressionNode;
    public Tokens Name { get; set; } = name;
    public Tokens Type { get; set; } = type;
    public bool IsExtern { get; set; } = isExtern;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
