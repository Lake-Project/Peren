using LLVMSharp.Interop;
using Lexxer;

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
            context.AddToTypeCheckerType(node.IntType);
            if (node.IntType == LLVMTypeRef.Int32)
                return LLVM.ConstInt(LLVM.Int32Type(), (ulong)node.n, 1);
            else if (node.IntType == LLVMTypeRef.Int8)
                return LLVM.ConstInt(LLVM.Int8Type(), (ulong)node.n, 1);
            else if (node.IntType == LLVMTypeRef.Int1)
                return LLVM.ConstInt(LLVM.Int1Type(), (ulong)node.n, 1);
            else
                throw new Exception("unsupported intType");
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

            // return builder.BuildAdd(
            //     node.right.CodeGen(this, builder, module, context),
            //     node.left.CodeGen(this, builder, module, context),
            //     "addtmp"
            // );
            LLVMValueRef L = node.left.CodeGen(this, builder, module, context);
            LLVMValueRef R = node.right.CodeGen(this, builder, module, context);
            LLVMTypeRef LType = context.GetFromTypeChecker();
            LLVMTypeRef RType = context.GetFromTypeChecker();
            Console.WriteLine(LType);
            Console.WriteLine(RType);
            if (LType == RType)
                context.AddToTypeCheckerType(RType);

            if (LType != RType)
            {
                if (LType.IntWidth > RType.IntWidth)
                {
                    L = builder.BuildSExt(L, RType, "truncate");
                    context.AddToTypeCheckerType(RType);
                }
                else
                {
                    R = builder.BuildSExt(R, LType, "truncate");
                    context.AddToTypeCheckerType(LType);
                }
            }
            return node.token.tokenType switch
            {
                TokenType.ADDITION => builder.BuildAdd(L, R, "addtmp"),
                _ => throw new Exception("unsupported op")
            };
            // {
            //     TokenType.ADDITION
            //       => builder.BuildAdd(
            //           node.right.CodeGen(this, builder, module, context),
            //           node.left.CodeGen(this, builder, module, context),
            //           "addtmp"
            //       ),
            //     TokenType.SUBTRACTION
            //       => builder.BuildSub(
            //           node.right.CodeGen(this, builder, module, context),
            //           node.left.CodeGen(this, builder, module, context),
            //           "subtmp"
            //       ),
            //     TokenType.MULTIPLICATION
            //       => builder.BuildMul(
            //           node.right.CodeGen(this, builder, module, context),
            //           node.left.CodeGen(this, builder, module, context),
            //           "multmp"
            //       ),
            //     TokenType.DIVISION
            //       => builder.BuildSDiv(
            //           node.right.CodeGen(this, builder, module, context),
            //           node.left.CodeGen(this, builder, module, context),
            //           "divtmp"
            //       ),
            //     TokenType.MODULAS
            //       => builder.BuildURem(
            //           node.right.CodeGen(this, builder, module, context),
            //           node.left.CodeGen(this, builder, module, context),
            //           "modtmp"
            //       ),
            //     _ => throw new Exception("unsupported op")
            // };
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
        context.AddToTypeCheckerType(l.type);
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
        if (!AllowedTypes.Contains(fun.retType))
            throw new TypeAccessException("not allowed type " + fun.retType);
        context.AddToTypeCheckerType(fun.type);
        return builder.BuildCall2(fun.type, fun.ValueRef, node.Values, fun.f.name);
    }
}
