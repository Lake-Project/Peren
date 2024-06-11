using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ReturnNode(INode? expression) : StatementNode
{
    public INode? Expression { get; set; } = expression;

    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
