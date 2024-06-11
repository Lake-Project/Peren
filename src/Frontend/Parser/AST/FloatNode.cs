using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;
using LLVMSharp.Interop;

public class FloatNode(float n) : INode
{
    public float Value { get; set; } = n;

    public FloatNode(Tokens n)
        : this(float.Parse(n.buffer)) { }

    public T Visit<T>(ExpressionVisit<T> visit) => visit.Visit(this);
}
