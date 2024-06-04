using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class WhileLoopNode : StatementNode
{
    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }
}
