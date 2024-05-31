using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class FunctionNode : StatementNode
{
    public List<StatementNode?> statements;
    public bool isExtern;
    public List<VaraibleDeclarationNode> Parameters;
    public LLVMTypeRef retType;
    public Tokens returType;

    public Tokens name;
    public LLVMTypeRef[] paramTypes;

    public FunctionNode(
        Tokens name,
        List<VaraibleDeclarationNode> Parameters,
        LLVMTypeRef retType,
        Tokens returType,
        List<StatementNode?> statements,
        bool isExtern
    )
    {
        this.name = name;
        this.retType = retType;
        this.returType = returType;
        this.statements = statements;
        this.Parameters = Parameters;
        this.paramTypes = new LLVMTypeRef[Parameters.Count];
        for (int i = 0; i < Parameters.Count; i++)
        {
            paramTypes[i] = Parameters[i].typeRef;
        }

        this.returType = returType;
        this.isExtern = isExtern;
    }

    // public FunctionNode(Tokens name, List<INode?> statements)
    // {
    //     this.name = name.buffer;
    //     this.statements = statements;
    //     this.Parameters = new List<VaraibleDeclarationNode>();
    //     paramTypes = new LLVMTypeRef[0];
    // }


    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        context.AllocateScope();

        // return visitor.visit(this, builder, module);
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(retType, paramTypes, false);

        LLVMValueRef function = module.AddFunction(name.buffer, funcType);
        context.AddFunction(name.buffer, this, function, funcType, retType);
        if (isExtern)
        {
            function.Linkage = LLVMLinkage.LLVMExternalLinkage;
            return function;
        }

        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        context.CurrentRetType = retType;
        builder.PositionAtEnd(entry);
        for (int i = 0; i < Parameters.Count; i++)
        {
            Parameters[i].AddToScope(builder, context, function.GetParam((uint)i));
        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.

        for (int i = 0; i < statements.Count; i++)
        {
            statements[i].CodeGen(visitor, builder, module, context);
        }

        if (!context.GetRet())
        {
            if (retType == LLVMTypeRef.Void)
                builder.BuildRetVoid();
        }

        context.DeallocateScope();
        return function;
    }

    public override LacusType VisitSemanticAnaylsis(SemanticVisitor visitor)
    {
        return visitor.SemanticAccept(this);
    }

    public override void Visit(StatementVisit visitor)
    {
        visitor.Visit(this);
    }

    public void Transform(IOptimize optimizer, Context context)
    {
        throw new NotImplementedException();
    }
}
