using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class CharNode(char value) : INode
{
    public char Value { get; set; } = value;

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}
