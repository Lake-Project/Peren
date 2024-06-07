using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public struct LLVMVar(LLVMValueRef valueRef, LLVMTypeRef typeRef)
{
    public LLVMValueRef Value { get; set; } = valueRef;
    public LLVMTypeRef Type { get; set; } = typeRef;
};

public struct LLVMFunction(LLVMTypeRef functionType, LLVMTypeRef returnType, LLVMValueRef valueRef)
{
    public LLVMTypeRef returnType = returnType;
    public LLVMTypeRef FunctionType = functionType;
    public LLVMValueRef FunctionValue = valueRef;

    public LLVMBasicBlockRef retVoidblock;
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

public class LLVMStatementVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef)
    : StatementVisit
{
    private LLVMContext Context { get; } = new();
    private LLVMFunction _currentFunction;
    private bool _returns;

    public override void Visit(VaraibleDeclarationNode node)
    {
        var value = builderRef.BuildAlloca(ToLLVMType(node.Type), node.Name.buffer);
        Context.vars.AddValue(node.Name, new LLVMVar(value, ToLLVMType(node.Type)));
        if (node.ExpressionNode != null)
        {
            LLVMValueRef eq = node.ExpressionNode.Visit(
                new LLVMExprVisitor(Context, builderRef, moduleRef)
            );
            builderRef.BuildStore(eq, value);
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        LLVMVar a = Context.vars.GetValue(node.Name, node.ScopeLocation);
        builderRef.BuildStore(
            node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef)),
            a.Value
        );
    }

    public override void Visit(FunctionCallNode node)
    {
        LLVMFunction a = Context.functions.GetValue(node.Name);
        builderRef.BuildCall2(
            a.FunctionType,
            a.FunctionValue,
            node.ParamValues.Select(n =>
                    n.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef))
                )
                .ToArray(),
            "funcCall"
        );
    }

    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            ToLLVMType(node.RetType),
            node.Parameters //params
                .Select(n => ToLLVMType(n.Type)) //converts param types
                .ToArray(), //to an array
            false
        );
        // node.Parameters.ForEach(n => n.Visit(this));
        LLVMValueRef function = moduleRef.AddFunction(node.Name.buffer, funcType);
        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        _currentFunction = new LLVMFunction(funcType, ToLLVMType(node.RetType), function);
        Context.functions.AddValue(node.Name, _currentFunction);
        // if (currentFunction.returnType == LLVMTypeRef.Void)
        // {
        //     LLVMBasicBlockRef voids = function.AppendBasicBlock("return");
        //     currentFunction.retVoidblock = voids;
        //     _builderRef.PositionAtEnd(voids);
        //     _builderRef.BuildRetVoid();
        // }

        builderRef.PositionAtEnd(entry);
        Context.vars.AllocateScope();
        foreach (var (param, index) in node.Parameters.Select((param, index) => (param, index)))
        {
            var LLVMParam = function.GetParam((uint)index);
            var name = param.Name.buffer;
            LLVMParam.Name = name;
            Context.vars.AddValue(
                param.Name,
                new LLVMVar(
                    builderRef.BuildAlloca(ToLLVMType(param.Type), name),
                    ToLLVMType(param.Type)
                )
            );
            builderRef.BuildStore(
                function.GetParam((uint)index),
                Context.vars.GetValue(param.Name).Value
            );
        }

        node.Statements.ForEach(n => n.Visit(this));
        if (!_returns && _currentFunction.returnType == LLVMTypeRef.Void)
            builderRef.BuildRetVoid();
        Context.vars.DeallocateScope();
    }

    public override void Visit(ReturnNode node)
    {
        if (_currentFunction.returnType == LLVMTypeRef.Void)
        {
            builderRef.BuildRetVoid();
            _returns = true;
        }
        else
        {
            builderRef.BuildRet(
                node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef))
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
            return Context.types.GetValue(type).typeRef;
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
