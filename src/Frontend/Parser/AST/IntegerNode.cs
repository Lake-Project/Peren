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

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context scope
    )
    {
        return visitor.Visit(this, builder, module, scope);
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
