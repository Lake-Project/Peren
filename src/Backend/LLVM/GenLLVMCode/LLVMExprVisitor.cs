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
        throw new NotImplementedException();
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
            TokenType.MODULAS => _builderRef.BuildURem(L, R, "modtmp"),
            _ => throw new Exception("unsupported int op")
        };
    }

    public override LLVMValueRef Visit(VaraibleReferenceNode node)
    {
        throw new NotImplementedException();
    }

    public override LLVMValueRef Visit(BooleanExprNode node)
    {
        throw new NotImplementedException();
    }

    public override LLVMValueRef Visit(CharNode node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong)(node.value));
    }
}
