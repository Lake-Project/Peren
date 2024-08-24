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

public class LLVMContext(Dictionary<string, CompilerModule> modules)
{
    public Dictionary<string, CompilerModule> Modules = modules;

    public CompilerModule CurrentModule;

    // private CompileContext<LLVMVar> globalVars = new();
    private CompileContext<LLVMVar> vars = new();

    // private CompileContext<LLVMType> types = new();
    //
    // public LLVMContext(Dictionary<string, CompilerModule> modules)
    // {
    //     this.Modules = modules;
    //     // functions.AllocateScope();
    //     // types.AllocateScope();
    //     // vars.AllocateScope();
    // }

    public void AddVars(string name, LLVMVar var)
    {
        vars.Add(name, var);
    }

    public LLVMVar GetVar(string name)
    {
        if (vars.Values.ContainsKey(name))
        {
            return vars.Get(name);
        }
        else if (CurrentModule.Varaibles.ContainsKey(name))
        {
            return CurrentModule.Varaibles[name];
        }
        else
        {
            foreach (var module in CurrentModule.imports)
            {
                if (module.Varaibles.ContainsKey(name))
                {
                    return module.Varaibles[name];
                }
            }

            throw new Exception("errpr");
            // CurrentModule.imports.ForEach(n =>
            // {
            //     if(n.Varaibles.ContainsKey())
            // });
        }
    }

    public LLVMFunction GetFunction(string name)
    {
        if (CurrentModule.Functions.ContainsKey(name))
        {
            return CurrentModule.Functions[name];
        }
        else
        {
            foreach (var module in CurrentModule.imports)
            {
                if (module.Functions.ContainsKey(name))
                {
                    return module.Functions[name];
                }
            }

            throw new Exception("error");
            // CurrentModule.imports.ForEach(n =>
            // {
            //     if(n.Varaibles.ContainsKey())
            // });
        }
    }

    public LLVMType GetType(string name)
    {
        if (CurrentModule.Types.ContainsKey(name))
        {
            return CurrentModule.Types[name];
        }
        else
        {
            foreach (var module in CurrentModule.imports)
            {
                if (module.Types.ContainsKey(name))
                {
                    return module.Types[name];
                }
            }

            throw new Exception("errpr");
            // CurrentModule.imports.ForEach(n =>
            // {
            //     if(n.Varaibles.ContainsKey())
            // });
        }
    }

    public CompilerModule SetCurrent(string name)
    {
        CurrentModule = modules[name];
        return modules[name];
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

            Context.AddVars(node.Name.buffer, new LLVMVar(value, type));
        }
        else
        {
            var value = builderRef.BuildAlloca(type, node.Name.buffer);
            Context.AddVars(node.Name.buffer, new LLVMVar(value, type));
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
        LLVMVar a = Context.GetVar(node.Name.buffer);

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
        LLVMFunction a = Context.GetFunction(node.Name.buffer);
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
        var function = Context.GetFunction(node.Name.buffer);
        _currentFunction = function;
        LLVMBasicBlockRef entry = function.FunctionValue.AppendBasicBlock("entry");
        builderRef.PositionAtEnd(entry);
        foreach (var (param, index) in node.Parameters.Select((param, index) => (param, index)))
        {
            var llvmParam = function.FunctionValue.GetParam((uint)index);
            var name = param.Name.buffer;
            llvmParam.Name = name;
            Context.AddVars(
                param.Name.buffer,
                new LLVMVar(
                    builderRef.BuildAlloca(Compile.ToLLVMType(param.Type, Context), name),
                    Compile.ToLLVMType(param.Type, Context)
                )
            );
            builderRef.BuildStore(
                function.FunctionValue.GetParam((uint)index),
                Context.GetVar(param.Name.buffer).Value
            );
        }

        node.Statements.ForEach(n => n.Visit(this));
        if (!_returns && _currentFunction.returnType == LLVMTypeRef.Void)
            builderRef.BuildRetVoid();
        // Context.vars.DeallocateScope();
    }

    public override void Visit(ModuleNode moduleNode)
    {
        Context.SetCurrent(moduleNode.Name.buffer);
        moduleNode.FunctionNodes.ForEach(n => n.Visit(this));
        // base.Visit(moduleNode);
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