using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class IntegerNode : INode
{
    public int n;

    // public IntegerNode(int n)
    // {
    //     this.n = n;
    // }


    public IntegerNode(Tokens n)
    {
        this.n = int.Parse(n.buffer);
    }

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }

    public override string ToString()
    {
        return n.ToString();
    }
}
