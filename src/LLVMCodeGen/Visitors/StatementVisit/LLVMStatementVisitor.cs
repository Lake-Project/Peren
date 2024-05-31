using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public struct LLVMVar { };

public struct LLVMFunction { };

public struct LLVMType { };

public struct LLVMContext
{
    public SemanticContext<LLVMVar> vars = new();
    public SemanticContext<LLVMFunction> functions = new();
    public SemanticContext<LLVMType> types = new();

    public LLVMContext()
    {
        functions.AllocateScope();
        types.AllocateScope();
        vars.AllocateScope();
    }
}

public class LLVMStatementVisitor : StatementVisit
{
    private LLVMContext _context;
    private LLVMBuilderRef _builderRef;
    private LLVMModuleRef _moduleRef;

    public LLVMStatementVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef)
    {
        _builderRef = builderRef;
        _moduleRef = moduleRef;
        _context = new LLVMContext();
    }

    public override void Visit(VaraibleDeclarationNode node)
    {
        var b = _builderRef.BuildAlloca(node.typeRef, node.name.buffer);
        LLVMValueRef eq = node.ExpressionNode.Visit(
            new LLVMExprVisitor(_context, _builderRef, _moduleRef)
        );
        _builderRef.BuildStore(eq, b);
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            node.retType,
            new LLVMTypeRef[] { },
            false
        );
        LLVMValueRef function = _moduleRef.AddFunction(node.name.buffer, funcType);
        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        _builderRef.PositionAtEnd(entry);
        _context.vars.AllocateScope();
        // node.Parameters.ForEach(n => n.Visit(this));
        node.statements.ForEach(n => n.Visit(this));
        _context.vars.DeallocateScope();
        _builderRef.BuildRetVoid();
    }
}
