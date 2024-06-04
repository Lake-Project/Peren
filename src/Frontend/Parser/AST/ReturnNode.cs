using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ReturnNode : StatementNode
{
    public INode? expression;
    public LLVMTypeRef type;

    public ReturnNode(LLVMTypeRef type, INode Expression)
    {
        this.expression = Expression;
        this.type = type;
    }

    public ReturnNode(INode? Expression)
    {
        this.expression = Expression;
    }

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
