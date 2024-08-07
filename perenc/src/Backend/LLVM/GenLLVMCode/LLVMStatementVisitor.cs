using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
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

public struct LLVMType(
    LLVMTypeRef typeRef,
    List<VaraibleDeclarationNode> varaibleDeclarationNodes)
{
    public LLVMTypeRef Type { get; set; } = typeRef;

    public List<VaraibleDeclarationNode> Vars = varaibleDeclarationNodes;
};

public struct LLVMContext
{
    public CompileContext<LLVMVar> globalVars = new();
    public CompileContext<LLVMVar> vars = new();
    public CompileContext<LLVMFunction> functions = new();
    public CompileContext<LLVMType> types = new();

    public LLVMContext()
    {
        // functions.AllocateScope();
        // types.AllocateScope();
        // vars.AllocateScope();
    }
}

public class LLVMStatementVisitor(LLVMContext context, LLVMBuilderRef builderRef, LLVMModuleRef moduleRef)
    : StatementVisit
{
    private LLVMContext Context { get; set; } = context;
    private LLVMFunction _currentFunction;
    private bool _returns;

    public override void Visit(VaraibleDeclarationNode node)
    {
        var type = Compile.ToLLVMType(node.Type, Context);
        // if (Context.vars.Values == 0)
        // return;
        if (node is ArrayNode n)
        {
            var size = n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
            var value = builderRef.BuildArrayAlloca(type, size);
            
            Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
        }
        else
        {
            var value = builderRef.BuildAlloca(type, node.Name.buffer);
            Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
            {
                if (node.Expression != null)
                {
                    LLVMValueRef eq = node.Expression.Visit(
                        new LLVMExprVisitor(Context, builderRef, moduleRef)
                    );
                    builderRef.BuildStore(eq, value);
                }
            }
        }
    }

    public override void Visit(VaraibleReferenceStatementNode node)
    {
        LLVMVar a = Compile.GetVar(Context, node.Name.buffer);

        if (node is ArrayRefStatementNode arr)
        {
            var loc = builderRef.BuildInBoundsGEP2(a.Type, a.Value,
                new LLVMValueRef[]
                {
                    arr.Element.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef))
                });
            builderRef.BuildStore(
                node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef)),
                loc
            );
        }
        else
        {
            builderRef.BuildStore(
                node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef)),
                a.Value
            );
        }
    }

    public override void Visit(FunctionCallNode node)
    {
        LLVMFunction a = Context.functions.Get(node.Name.buffer);
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
        // LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
        //     Compile.ToLLVMType(node.RetType.Name, Context),
        //     node.Parameters //params
        //         .Select(n => Compile.ToLLVMType(n.Type, Context)) //converts param types
        //         .ToArray(), //to an array
        //     false
        // );
        // // node.Parameters.ForEach(n => n.Visit(this));
        // LLVMValueRef function = moduleRef.AddFunction(node.Name.buffer, funcType);
        // _currentFunction = new LLVMFunction(funcType, Compile.ToLLVMType(node.RetType.Name, Context), function);
        // Context.functions.Add(node.Name.buffer, _currentFunction);
        // if (node.AttributesTuple.isExtern)
        // {
        //     function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        //     return;
        // }
        if (node.AttributesTuple.isExtern)
            return;
        var function = Context.functions.Get(node.Name.buffer);
        _currentFunction = function;
        LLVMBasicBlockRef entry = function.FunctionValue.AppendBasicBlock("entry");
        builderRef.PositionAtEnd(entry);
        foreach (var (param, index) in node.Parameters.Select((param, index) => (param, index)))
        {
            var llvmParam = function.FunctionValue.GetParam((uint)index);
            var name = param.Name.buffer;
            llvmParam.Name = name;
            Context.vars.Add(
                param.Name.buffer,
                new LLVMVar(
                    builderRef.BuildAlloca(Compile.ToLLVMType(param.Type, Context), name),
                    Compile.ToLLVMType(param.Type, Context)
                )
            );
            builderRef.BuildStore(
                function.FunctionValue.GetParam((uint)index),
                Compile.GetVar(Context, name).Value
            );
        }

        node.Statements.ForEach(n => n.Visit(this));
        if (!_returns && _currentFunction.returnType == LLVMTypeRef.Void)
            builderRef.BuildRetVoid();
        // Context.vars.DeallocateScope();
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


    public override void Visit(ForLoopNode node)
    {
        var loopCond = _currentFunction.FunctionValue.AppendBasicBlock("loopCOnd");

        var loopBody = _currentFunction.FunctionValue.AppendBasicBlock("loopBody");
        var loopEnd = _currentFunction.FunctionValue.AppendBasicBlock("Loopend");

        // Context.vars.AllocateScope();
        node.Iterator.Visit(this);
        builderRef.BuildBr(loopCond);
        builderRef.PositionAtEnd(loopCond);
        var v = node.Expr.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
        builderRef.BuildCondBr(v, loopBody, loopEnd);
        builderRef.PositionAtEnd(loopBody);
        node.Statements.ForEach(n => n.Visit(this));
        node.Inc.Visit(this);
        builderRef.BuildBr(loopCond);
        // Context.vars.DeallocateScope();
        builderRef.PositionAtEnd(loopEnd);
    }

    public override void Visit(WhileLoopNode node)
    {
        var loopCond = _currentFunction.FunctionValue.AppendBasicBlock("while.cond");

        var loopBody = _currentFunction.FunctionValue.AppendBasicBlock("while.body");
        var loopEnd = _currentFunction.FunctionValue.AppendBasicBlock("while.end");
        builderRef.BuildBr(loopCond);
        builderRef.PositionAtEnd(loopCond);
        LLVMValueRef v = node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
        builderRef.BuildCondBr(v, loopBody, loopEnd);
        // Context.vars.AllocateScope();

        builderRef.PositionAtEnd(loopBody);
        node.StatementNodes.ForEach(n => n.Visit(this));
        builderRef.BuildBr(loopCond);

        // Context.vars.DeallocateScope();
        builderRef.PositionAtEnd(loopEnd);
    }

    public override void Visit(IfNode node)
    {
        LLVMValueRef v = node.Expression.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));

        // Context.vars.AllocateScope();
        if (node.ElseNode.StatementNodes.Count != 0)
        {
            var If = _currentFunction.FunctionValue.AppendBasicBlock("if.then");
            var Else = _currentFunction.FunctionValue.AppendBasicBlock("else");
            var after = _currentFunction.FunctionValue.AppendBasicBlock("if.after");
            // Context.vars.AllocateScope();
            builderRef.BuildCondBr(v, If, Else);
            builderRef.PositionAtEnd(If);

            node.StatementNodes.ForEach(n => n.Visit(this));
            builderRef.BuildBr(after);
            // Context.vars.DeallocateScope();
            // Context.vars.AllocateScope();
            builderRef.PositionAtEnd(Else);
            node.ElseNode.StatementNodes.ForEach(n => n.Visit(this));
            // Context.vars.DeallocateScope();
            builderRef.BuildBr(after);
            builderRef.PositionAtEnd(after);
        }
        else
        {
            LLVMBasicBlockRef If = _currentFunction.FunctionValue.AppendBasicBlock("if.then");
            LLVMBasicBlockRef After = _currentFunction.FunctionValue.AppendBasicBlock("if.after");
            builderRef.BuildCondBr(v, If, After);
            // Context.vars.AllocateScope();
            builderRef.PositionAtEnd(If);
            node.StatementNodes.ForEach(n => n.Visit(this));
            // Context.vars.DeallocateScope();
            builderRef.BuildBr(After);
            builderRef.PositionAtEnd(After);
        }
    }
    
}