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
        return LLVMValueRef.CreateConstInt(node.Range switch
        {
            Range.OneBit => LLVMTypeRef.Int1,
            Range.EightBit => LLVMTypeRef.Int8,
            Range.SixteenBit => LLVMTypeRef.Int16,
            Range.ThirtyTwoBit => LLVMTypeRef.Int32,
            Range.SixtyFourBit => LLVMTypeRef.Int64,
            _ => throw new Exception("erro")
        }, (ulong)node.Value);
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
        LLVMFunction function = context.functions.Get(node.Name.buffer);
        return builderRef.BuildCall2(
            function.FunctionType,
            function.FunctionValue,
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
                _ => throw new Exception($"not accepted float math op {node.Token}")
            };
        }

        return node.Token.tokenType switch
        {
            TokenType.ADDITION => builderRef.BuildAdd(L, R, "addtmp"),
            TokenType.SUBTRACTION => builderRef.BuildSub(L, R, "subtmp"),
            TokenType.MULTIPLICATION => builderRef.BuildMul(L, R, "multmp"),
            TokenType.DIVISION => (node.IsUnsignedExpr)
                ? builderRef.BuildUDiv(L, R, "divtmp")
                : builderRef.BuildSDiv(L, R, "divtmp"),
            TokenType.MODULAS => builderRef.BuildSRem(L, R, "modtmp"),
            TokenType.OR => builderRef.BuildOr(L, R, "or"),
            TokenType.XOR => builderRef.BuildXor(L, R, "xor"),
            TokenType.AND => builderRef.BuildAnd(L, R, "and"),
            TokenType.NOT => builderRef.BuildNot(L, "not"),
            TokenType.R_SHIFT => (node.IsUnsignedExpr)
                ? builderRef.BuildLShr(L, R, "usigned_bitshift")
                : builderRef.BuildAShr(L, R, "signed_bitshift"),
            TokenType.L_SHIFT => builderRef.BuildShl(L, R, "bitshift"),
            _ => throw new Exception($"not accepted int math op {node.Token}")
        };
    }

    public override LLVMValueRef Visit(VaraibleReferenceNode node)
    {
        LLVMVar a = Compile.GetVar(context, node.Name.buffer);
        if (node is ArrayRefNode arr)
        {
            var loc = builderRef.BuildInBoundsGEP2(a.Type, a.Value,
                new LLVMValueRef[]
                {
                    arr.Elem.Visit(new LLVMExprVisitor(context, builderRef, moduleRef))
                });
            return builderRef.BuildLoad2(a.Type, loc, node.Name.buffer);
        }

        return builderRef.BuildLoad2(a.Type, a.Value, node.Name.buffer);
    } //:3

    public override LLVMValueRef Visit(BooleanExprNode node)
    {
        LLVMValueRef L = node.Left.Visit(this);
        LLVMValueRef R = node.Right.Visit(this);
        if (!node.IsFloat)
            return node.Op.tokenType switch
            {
                TokenType.BOOL_EQ => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntEQ, L, R, "cmp"),
                TokenType.LT => (node.IsUnsigned)
                    ? builderRef.BuildICmp(LLVMIntPredicate.LLVMIntULT, L, R, "cmp")
                    : builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLT, L, R, "cmp"),
                TokenType.LTE => (node.IsUnsigned)
                    ? builderRef.BuildICmp(LLVMIntPredicate.LLVMIntULE, L, R, "cmp")
                    : builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLE, L, R, "cmp"),
                TokenType.GT => (node.IsUnsigned)
                    ? builderRef.BuildICmp(LLVMIntPredicate.LLVMIntUGT, L, R, "cmp")
                    : builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSGT, L, R, "cmp"),
                TokenType.GTE => (node.IsUnsigned)
                    ? builderRef.BuildICmp(LLVMIntPredicate.LLVMIntUGE, L, R, "cmp")
                    : builderRef.BuildICmp(LLVMIntPredicate.LLVMIntULT, L, R, "cmp"),
                TokenType.NOT_EQUALS => builderRef.BuildICmp(LLVMIntPredicate.LLVMIntNE, L, R, "cmp"),
                _ => throw new Exception($"not accepted float bool op {node.Op}")
            };
        return node.Op.tokenType switch
        {
            TokenType.BOOL_EQ => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ, L, R, "cmp"),
            TokenType.LT => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLT, L, R, "cmp"),
            TokenType.LTE => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLE, L, R, "cmp"),
            TokenType.GT => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGT, L, R, "cmp"),
            TokenType.GTE => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGE, L, R, "cmp"),
            TokenType.NOT_EQUALS => builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealONE, L, R, "cmp"),
            _ => throw new Exception($"not accepted Int bool op {node.Op}")
        };
    }

    public override LLVMValueRef Visit(CharNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong)(node.Value));
    }

    public override LLVMValueRef Visit(CastNode node)
    {
        var v = node.Expr.Visit(this);
        var TargetType = Compile.ToLLVMType(node.type, context);
        if (node.inferredtype == CastType.FLOAT)
        {
            return builderRef.BuildFPToSI(v, TargetType);
        }
        else if (node.inferredtype == CastType.INT)
        {
            return builderRef.BuildSIToFP(v, TargetType);
        }
        else if (node.inferredtype == CastType.SEXT)
        {
            return builderRef.BuildSExt(v, TargetType);
        }
        else if (node.inferredtype == CastType.TRUNCATE)
        {
            return builderRef.BuildTrunc(v, TargetType);
        }

        return builderRef.BuildTrunc(v, TargetType);
    }

    public override LLVMValueRef Visit(StringNode node)
    {
        var c = builderRef.BuildGlobalString(node.Value);
        return c;
        // return builderRef.BuildGEP2(LLVMTypeRef.CreateInt(LLVMTypeRef.Int32, (ulong)1) );
        // return builderRef.Build(LLVMTypeRef.Int8, LLVMVa);
    }
}