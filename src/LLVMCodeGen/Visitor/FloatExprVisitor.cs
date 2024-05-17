using LLVMSharp.Interop;
using Lexxer;

public class FloatExprVisitor : IVisitor
{
    public LLVMValueRef Visit(
        IntegerNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        throw new NotImplementedException();
        // return new IntegerExpressionVisitor().Visit(node, builder, module, context);
    }

    public LLVMValueRef Visit(
        FloatNode node,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        unsafe
        {
            return LLVM.ConstReal(LLVM.FloatType(), node.n);
        }
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

            return node.token.tokenType switch
            {
                TokenType.ADDITION
                  => builder.BuildFAdd(
                      node.right.CodeGen(this, builder, module, context),
                      node.left.CodeGen(this, builder, module, context),
                      "addtmp"
                  ),
                TokenType.SUBTRACTION
                  => builder.BuildFSub(
                      node.right.CodeGen(this, builder, module, context),
                      node.left.CodeGen(this, builder, module, context),
                      "subtmp"
                  ),
                TokenType.MULTIPLICATION
                  => builder.BuildFMul(
                      node.right.CodeGen(this, builder, module, context),
                      node.left.CodeGen(this, builder, module, context),
                      "multmp"
                  ),
                TokenType.DIVISION
                  => builder.BuildFDiv(
                      node.right.CodeGen(this, builder, module, context),
                      node.left.CodeGen(this, builder, module, context),
                      "divtmp"
                  ),
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
        Var l = context.GetVar(node.name);
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
