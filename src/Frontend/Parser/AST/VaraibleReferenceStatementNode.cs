using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : INode
{
    public INode expression;
    public string name;

    public VaraibleReferenceStatementNode(string name, INode expresion)
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
        Dictionary<LLVMTypeRef, IVisitor> visitors =
            new()
            {
                [LLVMTypeRef.Int32] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int16] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int8] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Int1] = new IntegerExpressionVisitor(),
                [LLVMTypeRef.Float] = new FloatExprVisitor(),
            };
        unsafe
        {
            Var b = context.GetVar(name);
            // var typeRef = LLVM.TypeOf(b);
            // LLVMTypeRef pointedType = LLVM.GetElementType(b.type);
            context.AddToTypeCheckerType(b.type);
            if (!visitors.ContainsKey(b.type))
                throw new Exception("typechecking error");
            LLVMValueRef eq = expression.CodeGen(visitors[b.type], builder, module, context);
            LLVMTypeRef type = context.GetFromTypeChecker();
            if (type != b.type)
                if (context.CurrentRetType.IntWidth <= type.IntWidth)
                    eq = builder.BuildTrunc(eq, b.type, "SET VAR");
                else
                    eq = builder.BuildSExt(eq, b.type, "SET VAR");
            return builder.BuildStore(eq, b.valueRef);
        }
        //     if (
        //         b.type == LLVMTypeRef.Int32
        //         || b.type == LLVMTypeRef.Int8
        //         || b.type == LLVMTypeRef.Int1
        //     )
        //     {
        //         return builder.BuildStore(
        //             expression.CodeGen(new IntegerExpressionVisitor(), builder, module, context),
        //             b.valueRef
        //         );
        //     }
        //     else
        //     {
        //         return builder.BuildStore(
        //             expression.CodeGen(new CodeGenVisitor(), builder, module, context),
        //             b.valueRef
        //         );
        //     }
        // }

        // throw new NotImplementedException();
    }
}
