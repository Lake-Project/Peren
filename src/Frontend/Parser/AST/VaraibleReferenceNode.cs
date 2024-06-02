using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceNode : INode
{
    public Tokens name;

    public int ScopeLocation { get; set; }

    public VaraibleReferenceNode(Tokens varName)
    {
        name = varName;
        ScopeLocation = -1;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // throw new NotImplementedException();
        return visitor.Visit(this, builder, module, context);
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
