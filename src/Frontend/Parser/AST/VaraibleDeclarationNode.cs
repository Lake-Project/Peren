using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleDeclarationNode : StatementNode
{
    public INode ExpressionNode;
    public Tokens name;
    public Tokens type;
    public bool isExtern;
    public bool isStruct;

    public VaraibleDeclarationNode(Tokens type, Tokens name, INode ExpressionNode, bool isExtern)
    {
        this.ExpressionNode = ExpressionNode;
        this.name = name;
        this.type = type;
        this.isExtern = isExtern;
    }

    //
    public override void Visit(StatementVisit visitor)
    {
        // base.Visit(visitor);
        visitor.Visit(this);
    }
}
