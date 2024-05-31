using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceNode : INode
{
    public Tokens name;

    public VaraibleReferenceNode(Tokens varName)
    {
        name = varName;
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

    public LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return name.ToString();
    }
}
