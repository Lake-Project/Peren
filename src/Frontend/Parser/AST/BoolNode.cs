using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

public class BoolNode(bool value) : INode
{
    public bool Value { get; set; } = value;
    private INode _nodeImplementation;

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}
