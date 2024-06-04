using System.Linq.Expressions;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceNode : INode
{
    public int ScopeLocation;
    public Tokens name;

    public VaraibleReferenceNode(Tokens varName)
    {
        name = varName;
    }

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }

    public override string ToString()
    {
        return name.ToString();
    }
}
