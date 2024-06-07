using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMExprVisitor(
    LLVMContext context,
    LLVMBuilderRef builderRef,
    LLVMModuleRef moduleRef
) : ExpressionVisit<LLVMValueRef>
{
    public override LLVMValueRef Visit(IntegerNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong)node.Value);
    }

    public override LLVMValueRef Visit(FloatNode node)
    {
        return LLVMValueRef.CreateConstReal(LLVMTypeRef.Float, node.Value);
    }

    public override LLVMValueRef Visit(BoolNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, (ulong)((node.Value) ? 1 : 0));
    }

    public override LLVMValueRef Visit(FunctionCallNode node)
    {
        LLVMFunction a = context.functions.GetValue(node.Name);
        return builderRef.BuildCall2(
            a.FunctionType,
            a.FunctionValue,
            node.ParamValues.Select(n =>
                    n.Visit(new LLVMExprVisitor(context, builderRef, moduleRef))
                ) //oprams
                .ToArray(), //params
            "funcCall"
        );
    }

    public override LLVMValueRef Visit(OpNode node)
    {
        LLVMValueRef L = node.Left.Visit(this);
        LLVMValueRef R = node.Right.Visit(this);
        if (node.FloatExpr)
        {
            return node.Token.tokenType switch
            {
                TokenType.ADDITION => builderRef.BuildFAdd(L, R, "addtmp"),
                TokenType.SUBTRACTION => builderRef.BuildFSub(L, R, "subtmp"),
                TokenType.MULTIPLICATION => builderRef.BuildFMul(L, R, "multmp"),
                TokenType.DIVISION => builderRef.BuildFDiv(L, R, "divtmp"),
                _ => throw new Exception("unsupported float op")
            };
        }

        return node.Token.tokenType switch
        {
            TokenType.ADDITION => builderRef.BuildAdd(L, R, "addtmp"),
            TokenType.SUBTRACTION => builderRef.BuildSub(L, R, "subtmp"),
            TokenType.MULTIPLICATION => builderRef.BuildMul(L, R, "multmp"),
            TokenType.DIVISION => builderRef.BuildSDiv(L, R, "divtmp"),
            TokenType.OR => builderRef.BuildOr(L, R, "modtmp"),
            TokenType.AND => builderRef.BuildAnd(L, R, "modtmp"),
            TokenType.R_SHIFT => builderRef.BuildLShr(L, R, "bitshift"),
            TokenType.L_SHIFT => builderRef.BuildShl(L, R, "bitshift"),
            _ => throw new Exception("unsupported int op")
        };
    }

    public override LLVMValueRef Visit(VaraibleReferenceNode node)
    {
        LLVMVar a = context.vars.GetValue(node.Name, node.ScopeLocation);
        return builderRef.BuildLoad2(a.Type, a.Value, node.Name.buffer);
    }

    public override LLVMValueRef Visit(BooleanExprNode node)
    {
        LLVMValueRef L = node.left.Visit(this);
        LLVMValueRef R = node.right.Visit(this);
        if (!node.isFloat)
            return node.op.tokenType switch
            {
                TokenType.BOOL_EQ => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntEQ, L, R, "cmp"),
                TokenType.LT => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLT, L, R, "cmp"),
                TokenType.LTE => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLE, L, R, "cmp"),
                TokenType.GT => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSGT, L, R, "cmp"),
                TokenType.GTE => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSGE, L, R, "cmp"),

                _ => throw new Exception("not accepted op")
            };
        return node.op.tokenType switch
        {
            TokenType.BOOL_EQ => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ, L, R, "cmp"),
            TokenType.LT => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLT, L, R, "cmp"),
            TokenType.LTE => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLE, L, R, "cmp"),
            TokenType.GT => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGT, L, R, "cmp"),
            TokenType.GTE => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGE, L, R, "cmp"),
            _ => throw new Exception("not accepted op")
        };
    }

    public override LLVMValueRef Visit(CharNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong)(node.Value));
    }
}
