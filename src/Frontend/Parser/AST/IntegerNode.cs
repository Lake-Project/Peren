using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class IntegerNode : INode
{
    public int n;
    public LLVMTypeRef IntType;

    // public IntegerNode(int n)
    // {
    //     this.n = n;
    // }

    public IntegerNode(int n, LLVMTypeRef type)
    {
        this.n = n;
        this.IntType = type;
    }

    public IntegerNode(Tokens n, LLVMTypeRef type)
    {
        this.n = int.Parse(n.buffer);
        this.IntType = type;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context scope
    )
    {
        return visitor.Visit(this, builder, module, scope);

        // throw new NotImplementedException();
    }

    public LLVMValueRef Visit(ExpressionVisit visit)
    {
        return visit.Visit(this);
    }

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        return visitor.SemanticAccept(this);
    }

    public override string ToString()
    {
        return n.ToString();
    }
}
