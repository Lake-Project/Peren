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

public struct LLVMFunction
{
    public LLVMTypeRef returnType;
    public LLVMTypeRef FunctionType;
    public LLVMValueRef FunctionValue;

    public LLVMBasicBlockRef retVoidblock;

    public LLVMFunction(LLVMTypeRef functionType, LLVMTypeRef typeRef, LLVMValueRef valueRef)
    {
        this.returnType = typeRef;
        this.FunctionValue = valueRef;
        this.FunctionType = functionType;
    }
};

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
    private LLVMFunction currentFunction;
    private bool returns;

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
        LLVMFunction a = _context.functions.GetValue(node.Name);
        _builderRef.BuildCall2(
            a.FunctionType,
            a.FunctionValue,
            node.ParamValues.Select(n =>
                    n.Visit(new LLVMExprVisitor(_context, _builderRef, _moduleRef))
                )
                .ToArray(),
            "funcCall"
        );
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
        currentFunction = new LLVMFunction(funcType, ToLLVMType(node.retType), function);
        _context.functions.AddValue(node.name, currentFunction);
        // if (currentFunction.returnType == LLVMTypeRef.Void)
        // {
        //     LLVMBasicBlockRef voids = function.AppendBasicBlock("return");
        //     currentFunction.retVoidblock = voids;
        //     _builderRef.PositionAtEnd(voids);
        //     _builderRef.BuildRetVoid();
        // }

        _builderRef.PositionAtEnd(entry);
        _context.vars.AllocateScope();
        foreach (var (param, index) in node.Parameters.Select((param, index) => (param, index)))
        {
            var llvmParam = function.GetParam((uint)index);
            var name = param.name.buffer;
            llvmParam.Name = name;
            _context.vars.AddValue(
                param.name,
                new LLVMVar(
                    _builderRef.BuildAlloca(ToLLVMType(param.type), name),
                    ToLLVMType(param.type)
                )
            );
            _builderRef.BuildStore(
                function.GetParam((uint)index),
                _context.vars.GetValue(param.name).Value
            );
        }

        node.statements.ForEach(n => n.Visit(this));
        if (!returns && currentFunction.returnType == LLVMTypeRef.Void)
            _builderRef.BuildRetVoid();
        _context.vars.DeallocateScope();
    }

    public override void Visit(ReturnNode node)
    {
        if (currentFunction.returnType == LLVMTypeRef.Void)
        {
            _builderRef.BuildRetVoid();
            returns = true;
        }
        else
        {
            _builderRef.BuildRet(
                node.expression.Visit(new LLVMExprVisitor(_context, _builderRef, _moduleRef))
            );
        }
    }

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
