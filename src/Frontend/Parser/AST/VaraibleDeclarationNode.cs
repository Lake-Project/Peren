using System.Linq.Expressions;
using LLVMSharp.Interop;

public class VaraibleDeclarationNode : INode
{
    public INode? ExpressionNode;
    public LLVMTypeRef typeRef;
    public string name;
    public bool isExtern;

    public VaraibleDeclarationNode(
        LLVMTypeRef type,
        string name,
        INode? ExpressionNode,
        bool isExtern
    )
    {
        this.ExpressionNode = ExpressionNode;
        this.typeRef = type;
        this.name = name;
        this.isExtern = isExtern;
    }

    public void AddToScope(LLVMBuilderRef builder, Context context, LLVMValueRef value)
    {
        // LLVMValueRef b = builder.BuildAlloca(typeRef, name);
        context.AddNewVar(typeRef, name, builder.BuildAlloca(typeRef, name));
        Var l = context.GetVar(name);
        builder.BuildStore(value, l.valueRef);
    }

    // public VaraibleDeclarationNode()
    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        Dictionary<LLVMTypeRef, IVisitor> visitors =
            new()
            {
                [LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Float] = new FloatExprVisitor(),
            };
        LLVMValueRef b;
        if (!visitors.ContainsKey(typeRef))
        {
            // LLVM.StructType()
            throw new Exception("a");
        }
        if (context.ScopeSize() == 0)
            b = module.AddGlobal(typeRef, name);
        else
            b = builder.BuildAlloca(typeRef, name);
        context.AddNewVar(typeRef, name, b);
        if (isExtern)
        {
            b.Linkage = LLVMLinkage.LLVMExternalLinkage;
            return b;
        }
        if (ExpressionNode == null)
            return b;
        LLVMValueRef eq = ExpressionNode.CodeGen(visitors[typeRef], builder, module, context);
        LLVMTypeRef type = context.GetFromTypeChecker();
        if (type != typeRef)
            if (context.CurrentRetType.IntWidth <= type.IntWidth)
                eq = builder.BuildTrunc(eq, typeRef, "SET VAR");
            else
                eq = builder.BuildSExt(eq, typeRef, "SET VAR");
        if (context.ScopeSize() == 0)
        {
            unsafe
            {
                // LLVM.SetLinkage(b, LLVMLinkage.LLVMExternalLinkage);
                LLVM.SetInitializer(b, eq); // Initialize the global variable with value 42
                return b;
            }
        }
        else
        {
            return builder.BuildStore(eq, b);
        }
        // if (context.ScopeSize() == 0)
        // {
        //     b = module.AddGlobal(typeRef, name);
        //     // b = builder.Build(typeRef, name);
        //     context.AddNewVar(typeRef, name, b);
        //     if (isExtern)
        //     {
        //         b.Linkage = LLVMLinkage.LLVMExternalLinkage;
        //     }

        //     if (
        //         (
        //             typeRef == LLVMTypeRef.Int32
        //             || typeRef == LLVMTypeRef.Int8
        //             || typeRef == LLVMTypeRef.Int1
        //         )
        //         && ExpressionNode != null
        //     )
        //     {
        //         unsafe
        //         {
        //             // LLVM.SetLinkage(b, LLVMLinkage.LLVMExternalLinkage);
        //             LLVM.SetInitializer(
        //                 b,
        //                 ExpressionNode.CodeGen(
        //                     new IntegerExpressionVisitor(),
        //                     builder,
        //                     module,
        //                     context
        //                 )
        //             ); // Initialize the global variable with value 42
        //         }
        //     }
        //     return b;
        // }
        // else
        // {
        //     // Console.WriteLine(context.ScopeSize());
        //     b = builder.BuildAlloca(typeRef, name);
        // }
        // context.AddNewVar(typeRef, name, b);
        // context.AddToTypeCheckerType(typeRef);
        // if (
        //     (
        //         typeRef == LLVMTypeRef.Int32
        //         || typeRef == LLVMTypeRef.Int8
        //         || typeRef == LLVMTypeRef.Int1
        //     )
        //     && ExpressionNode != null
        // )
        // {
        //     return builder.BuildStore(
        //         ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, context),
        //         b
        //     );
        // }
        // if (typeRef == LLVMTypeRef.Float && ExpressionNode != null)
        // {
        //     return builder.BuildStore(
        //         ExpressionNode.CodeGen(new FloatExprVisitor(), builder, module, context),
        //         b
        //     );
        // }

        // return b;
    }
}
