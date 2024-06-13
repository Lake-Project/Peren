using LacusLLVM.Frontend.Parser.AST;
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
        if (Context.vars.GetSize() == 0)
            return;
        var value = builderRef.BuildAlloca(ToLLVMType(node.Type), node.Name.buffer);
        Context.vars.AddValue(node.Name, new LLVMVar(value, ToLLVMType(node.Type)));
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
        _currentFunction = new LLVMFunction(funcType, ToLLVMType(node.RetType), function);
        Context.functions.AddValue(node.Name, _currentFunction);
        if (node.IsExtern)
        {
            function.Linkage = LLVMLinkage.LLVMExternalLinkage;
            return;
        }

        LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
        builderRef.PositionAtEnd(entry);
        Context.vars.AllocateScope();
        foreach (var (param, index) in node.Parameters.Select((param, index) => (param, index)))
        {
            var llvmParam = function.GetParam((uint)index);
            var name = param.Name.buffer;
            llvmParam.Name = name;
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
                (node.Expression ?? throw new Exception("null return ")).Visit(
                    new LLVMExprVisitor(Context, builderRef, moduleRef)
                )
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
        var loopBody = _currentFunction.FunctionValue.AppendBasicBlock("loop.body");
        var loopEnd = _currentFunction.FunctionValue.AppendBasicBlock("if.then");

        LLVMValueRef v = node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
        // builderRef.BuildCondBr(loopCond, loopBody, loopEnd);

        Context.vars.AllocateScope();
        builderRef.PositionAtEnd(loopBody);
        node.StatementNodes.ForEach(n => n.Visit(this));
        Context.vars.DeallocateScope();
        builderRef.PositionAtEnd(loopEnd);
    }

    public override void Visit(IfNode node)
    {
        LLVMValueRef v = node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));

        Context.vars.AllocateScope();
        if (node.ElseNode.StatementNodes.Count != 0)
        {
            LLVMBasicBlockRef If = _currentFunction.FunctionValue.AppendBasicBlock("if.then");
            LLVMBasicBlockRef Else = _currentFunction.FunctionValue.AppendBasicBlock("else");
            LLVMBasicBlockRef after = _currentFunction.FunctionValue.AppendBasicBlock("if.after");
            Context.vars.AllocateScope();
            builderRef.BuildCondBr(v, If, Else);
            builderRef.PositionAtEnd(If);

            node.StatementNodes.ForEach(n => n.Visit(this));
            builderRef.BuildBr(after);
            Context.vars.DeallocateScope();
            Context.vars.AllocateScope();
            builderRef.PositionAtEnd(Else);
            node.ElseNode.StatementNodes.ForEach(n => n.Visit(this));
            Context.vars.DeallocateScope();
            builderRef.BuildBr(after);
            builderRef.PositionAtEnd(after);
        }
        else
        {
            LLVMBasicBlockRef If = _currentFunction.FunctionValue.AppendBasicBlock("if.then");
            LLVMBasicBlockRef After = _currentFunction.FunctionValue.AppendBasicBlock("if.after");
            builderRef.BuildCondBr(v, If, After);
            Context.vars.AllocateScope();
            builderRef.PositionAtEnd(If);
            node.StatementNodes.ForEach(n => n.Visit(this));
            Context.vars.DeallocateScope();
            builderRef.BuildBr(After);
            builderRef.PositionAtEnd(After);
        }
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