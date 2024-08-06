using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMTopLevelVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef) : TopLevelVisitor
{
    private LLVMContext Context { get; } = new();

    public override void Visit(VaraibleDeclarationNode node)
    {
        var type = Compile.ToLLVMType(node.Type, Context);
        if (node is ArrayNode n)
        {
            // var size = n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
            // var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
            // value.Initializer = 
            ///TODO: come up with a plan for arrays
            throw new NotImplementedException();
        }
        else
        {
            var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // var value = builderRef.BuildAlloca(type, node.Name.buffer);
            if (node.AttributesTuple.isExtern)
                value.Linkage = LLVMLinkage.LLVMExternalLinkage;
            Context.globalVars.Add(node.Name.buffer, new LLVMVar(value, type));
            {
                if (node.Expression != null)
                {
                    LLVMValueRef eq = node.Expression.Visit(
                        new LLVMExprVisitor(Context, builderRef, moduleRef)
                    );
                    value.Initializer = eq;
                }
                else
                {
                    if (!node.AttributesTuple.isExtern)
                        value.Initializer = LLVMValueRef.CreateConstNull(type);
                }
            }
        }
    }

    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            Compile.ToLLVMType(node.RetType.Name, Context),
            node.Parameters //params
                .Select(n => Compile.ToLLVMType(n.Type, Context)) //converts param types
                .ToArray(), //to an array
            false
        );
        // node.Parameters.ForEach(n => n.Visit(this));
        LLVMValueRef function = moduleRef.AddFunction(node.Name.buffer, funcType);
        Context.functions.Add(node.Name.buffer,
            new LLVMFunction(funcType, Compile.ToLLVMType(node.RetType.Name, Context), function));
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
    }

    public override void Visit(ModuleNode node)
    {
        node.FunctionNodes.ForEach(n => n.Visit(this));
        node.StructNodes.ForEach(n => n.Visit(this));
        node.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
        node.FunctionNodes.ForEach(n => n.Visit(new LLVMStatementVisitor(Context, builderRef, moduleRef)));
        // node.StatementNodes.ForEach(n => n.Visit(this));
    }

    public override void Visit(TopLevelStatement node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(StructNode node)
    {
        var llvmstruct = moduleRef.Context.CreateNamedStruct(node.Name.buffer);
        Context.types.Add(node.Name.buffer, new LLVMType(
            llvmstruct, node.Vars));
    }
}