using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class ForLoopNode : StatementNode
{
    public override void Visit(StatementVisit visitor) => visitor.Visit(this);
}
