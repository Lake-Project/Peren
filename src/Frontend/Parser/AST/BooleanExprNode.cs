using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class BooleanExprNode(INode left, INode right, Tokens op) : INode
{
    public INode left = left;
    public INode right = right;
    public Tokens op = op;
    public bool isFloat;

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}
