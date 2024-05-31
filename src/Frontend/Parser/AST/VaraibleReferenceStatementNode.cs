using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : StatementNode
{
    public INode expression;
    public Tokens name;
    public int ScopeLocation { get; set; }

    public VaraibleReferenceStatementNode(Tokens name, INode expresion)
    {
        this.name = name;
        this.expression = expresion;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        unsafe
        {
            Var b = context.GetVar(name);
            LLVMValueRef eq = context.HandleTypes(b.type, builder, module, expression);
            // if (eq.IsConstant)
            // {
            //     context.UpdateConst(name, eq);
            //     return b.constant;
            // }
            // else if (b.IsConstant)
            //     context.removeConst(name);
            return builder.BuildStore(eq, b.valueRef);
        }
    }

    public override LLVMValueRef Visit(ExpressionVisit visit)
    {
        throw new NotImplementedException();
    }

    public override LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        return visitor.SemanticAccept(this);
    }
}
