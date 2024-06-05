using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public struct LLVMVar
{
    public LLVMValueRef Value { get; set; }
    public LLVMTypeRef Type { get; set; }

    public LLVMVar(LLVMValueRef valueRef, LLVMTypeRef typeRef)
    {
        Value = valueRef;
        Type = typeRef;
    }
};

public struct LLVMFunction { };

public struct LLVMType
{
    public LLVMTypeRef typeRef { get; set; }
};

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
        var value = _builderRef.BuildAlloca(ToLLVMType(node.type), node.name.buffer);
        _context.vars.AddValue(node.name, new LLVMVar(value, ToLLVMType(node.type)));
        if (node.ExpressionNode != null)
        {
            LLVMValueRef eq = node.ExpressionNode.Visit(
                new LLVMExprVisitor(_context, _builderRef, _moduleRef)
            );
            _builderRef.BuildStore(eq, value);
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        LLVMVar a = _context.vars.GetValue(node.name, node.ScopeLocation);
        _builderRef.BuildStore(
            node.expression.Visit(new LLVMExprVisitor(_context, _builderRef, _moduleRef)),
            a.Value
        );
    }

    public override void Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            ToLLVMType(node.retType),
            node.Parameters //params
                .Select(n => ToLLVMType(n.type)) //converts param types
                .ToArray(), //to an array
            false
        );
        // node.Parameters.ForEach(n => n.Visit(this));
        LLVMValueRef function = _moduleRef.AddFunction(node.name.buffer, funcType);
        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        _builderRef.PositionAtEnd(entry);
        _context.vars.AllocateScope();
        node.Parameters.ForEach(n => n.Visit(this));
        node.statements.ForEach(n => n.Visit(this));
        _context.vars.DeallocateScope();
        _builderRef.BuildRetVoid();
    }

    public override void Visit(ReturnNode node) { }

    public override void Visit(CastNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ForLoopNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(WhileLoopNode node)
    {
        throw new NotImplementedException();
    }

    public LLVMTypeRef ToLLVMType(Tokens type)
    {
        if (type.tokenType == TokenType.WORD)
        {
            return _context.types.GetValue(type).typeRef;
        }

        return type.tokenType switch
        {
            TokenType.INT => LLVMTypeRef.Int32,
            TokenType.FLOAT => LLVMTypeRef.Float,
            TokenType.BOOL => LLVMTypeRef.Int1,
            TokenType.CHAR => LLVMTypeRef.Int8,
            TokenType.VOID => LLVMTypeRef.Void,
            _ => throw new Exception($"undefined {type.ToString()} type")
        };
    }
}
