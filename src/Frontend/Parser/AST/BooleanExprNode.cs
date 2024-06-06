using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.Frontend.Parser.AST;

public class BooleanExprNode : INode
{
    public INode left;
    public INode right;
    public Tokens op;
    public bool isFloat;

    public BooleanExprNode(INode left, INode right, Tokens op)
    {
        this.left = left;
        this.right = right;
        this.op = op;
    }

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}
