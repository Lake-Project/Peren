using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMExprVisitor : ExpressionVisit<LLVMValueRef>
{
    private LLVMContext _context;
    private LLVMBuilderRef _builderRef;
    private LLVMModuleRef _moduleRef;

    public LLVMExprVisitor(LLVMContext context, LLVMBuilderRef builderRef, LLVMModuleRef moduleRef)
    {
        this._builderRef = builderRef;
        this._moduleRef = moduleRef;
        this._context = context;
    }

    public override LLVMValueRef Visit(IntegerNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong)node.n);
    }

    public override LLVMValueRef Visit(FloatNode node)
    {
        return LLVMValueRef.CreateConstReal(LLVMTypeRef.Float, node.n);
    }

    public override LLVMValueRef Visit(BoolNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, (ulong)((node.value) ? 1 : 0));
    }

    public override LLVMValueRef Visit(FunctionCallNode node)
    {
        LLVMFunction a = _context.functions.GetValue(node.Name);
        return _builderRef.BuildCall2(
            a.FunctionType,
            a.FunctionValue,
            node.ParamValues.Select(n =>
                    n.Visit(new LLVMExprVisitor(_context, _builderRef, _moduleRef))
                ) //oprams
                .ToArray(), //params
            "funcCall"
        );
    }

    public override LLVMValueRef Visit(OpNode node)
    {
        LLVMValueRef L = node.left.Visit(this);
        LLVMValueRef R = node.right.Visit(this);
        if (node.FloatExpr)
        {
            return node.token.tokenType switch
            {
                TokenType.ADDITION => _builderRef.BuildFAdd(L, R, "addtmp"),
                TokenType.SUBTRACTION => _builderRef.BuildFSub(L, R, "subtmp"),
                TokenType.MULTIPLICATION => _builderRef.BuildFMul(L, R, "multmp"),
                TokenType.DIVISION => _builderRef.BuildFDiv(L, R, "divtmp"),
                _ => throw new Exception("unsupported float op")
            };
        }

        return node.token.tokenType switch
        {
            TokenType.ADDITION => _builderRef.BuildAdd(L, R, "addtmp"),
            TokenType.SUBTRACTION => _builderRef.BuildSub(L, R, "subtmp"),
            TokenType.MULTIPLICATION => _builderRef.BuildMul(L, R, "multmp"),
            TokenType.DIVISION => _builderRef.BuildSDiv(L, R, "divtmp"),
            TokenType.OR => _builderRef.BuildOr(L, R, "modtmp"),
            TokenType.AND => _builderRef.BuildAnd(L, R, "modtmp"),
            TokenType.R_SHIFT => _builderRef.BuildLShr(L, R, "bitshift"),
            TokenType.L_SHIFT => _builderRef.BuildShl(L, R, "bitshift"),
            _ => throw new Exception("unsupported int op")
        };
    }

    public override LLVMValueRef Visit(VaraibleReferenceNode node)
    {
        LLVMVar a = _context.vars.GetValue(node.name, node.ScopeLocation);
        return _builderRef.BuildLoad2(a.Type, a.Value);
    }

    public override LLVMValueRef Visit(BooleanExprNode node)
    {
        LLVMValueRef L = node.left.Visit(this);
        LLVMValueRef R = node.right.Visit(this);
        if (!node.isFloat)
            return node.op.tokenType switch
            {
                TokenType.BOOL_EQ => _builderRef.BuildICmp(LLVMIntPredicate.LLVMIntEQ, L, R, "cmp"),
                TokenType.LT => _builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLT, L, R, "cmp"),
                TokenType.LTE => _builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSLE, L, R, "cmp"),
                TokenType.GT => _builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSGT, L, R, "cmp"),
                TokenType.GTE => _builderRef.BuildICmp(LLVMIntPredicate.LLVMIntSGE, L, R, "cmp"),

                _ => throw new Exception("not accepted op")
            };
        return node.op.tokenType switch
        {
            TokenType.BOOL_EQ => _builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ, L, R, "cmp"),
            TokenType.LT => _builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLT, L, R, "cmp"),
            TokenType.LTE => _builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOLE, L, R, "cmp"),
            TokenType.GT => _builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGT, L, R, "cmp"),
            TokenType.GTE => _builderRef.BuildFCmp(LLVMRealPredicate.LLVMRealOGE, L, R, "cmp"),
            _ => throw new Exception("not accepted op")
        };
    }

    public override LLVMValueRef Visit(CharNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong)(node.value));
    }
}
