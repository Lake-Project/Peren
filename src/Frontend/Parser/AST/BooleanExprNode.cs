using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class BooleanExprNode(INode left, INode right, Tokens op) : INode
{
    public INode Left { get; set; } = left;
    public INode Right { get; set; } = right;
    public Tokens Op { get; set; } = op;
    public bool IsFloat;

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
