using Lexxer;
using LLVMSharp.Interop;

public class IntegerExpressionVisitor : IVisitor
{
    private HashSet<LLVMTypeRef> AllowedTypes = new HashSet<LLVMTypeRef>
    {
        LLVMTypeRef.Int32,
        LLVMTypeRef.Int8,
        LLVMTypeRef.Int1
    };

    public IntegerExpressionVisitor() { }

    public LLVMValueRef Visit(
        IntegerNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        unsafe
        {
            return LLVM.ConstInt(node.IntType, (ulong)node.n, 1);
        }
    }

    public LLVMValueRef Visit(
        FloatNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new TypeMisMatchException();
    }

    public LLVMValueRef Visit(
        OpNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        if (node.left == null && node.right != null)
        {
            return node.right.CodeGen(this, builder, module, context);
        }
        else if (node.right == null && node.left != null)
        {
            return node.left.CodeGen(this, builder, module, context);
        }
        else if (node.right == null && node.left == null)
        {
            unsafe
            {
                return LLVM.ConstInt(LLVM.Int32Type(), (ulong)0, 1);
            }
        }
        else
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            LLVMValueRef L = node.left.CodeGen(this, builder, module, context);
            LLVMValueRef R = node.right.CodeGen(this, builder, module, context);
            return node.token.tokenType switch
            {
                TokenType.ADDITION => builder.BuildAdd(L, R, "addtmp"),
                TokenType.SUBTRACTION => builder.BuildSub(L, R, "subtmp"),
                TokenType.MULTIPLICATION => builder.BuildMul(L, R, "multmp"),
                TokenType.DIVISION => builder.BuildSDiv(L, R, "divtmp"),
                TokenType.MODULAS => builder.BuildURem(L, R, "modtmp"),
                _ => throw new Exception("unsupported op")
            };
        }
    }

    public LLVMValueRef Visit(
        VaraibleReferenceNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // throw new NotImplementedException();
        Var l = context.GetVar(node.name);

        if (!AllowedTypes.Contains(l.type))
            throw new TypeAccessException("not allowed type");
        if (l.IsConstant)
            return l.constant;
        return builder.BuildLoad2(l.type, l.valueRef);
    }

    public LLVMValueRef Visit(
        FunctionCallNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        Function fun = context.GetFunction(node.Name);
        return builder.BuildCall2(fun.type, fun.ValueRef, node.Values, fun.f.name.buffer);
    }
}
