using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;
using LLVMSharp.Interop;

public class FloatNode : INode
{
    public float n;

    public FloatNode(float n)
    {
        this.n = n;
    }

    public FloatNode(Tokens n)
    {
        this.n = float.Parse(n.buffer);
    }

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }
}
