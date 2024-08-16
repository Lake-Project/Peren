using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMVaraibles(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef, LLVMContext context) : StatementVisit
{
    public CompilerModule CurrentModule = new();

    public override void Visit(VaraibleDeclarationNode node)
    {
        var type = Compile.ToLLVMType(node.Type, context);
        if (node is ArrayNode n)
        {
            // var size = n.Size.Visit(new LLVMExprVisitor(Context, builderRef, moduleRef));
            // var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // Context.vars.Add(node.Name.buffer, new LLVMVar(value, type));
            // value.Initializer = 
            ///TODO: come up with a plan for arrays
            // throw new NotImplementedException();
            var value = moduleRef.AddGlobal(type, node.Name.buffer);

            value.Initializer = builderRef.BuildArrayAlloca(type,
                n.Size.Visit(new LLVMExprVisitor(context, builderRef, moduleRef)),
                "test");
            // LLVMTypeRef.CreateArray()
            // Context.globalVars.Add(node.Name.buffer, new LLVMVar(value, type));
            CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));

            // LLVMTypeRef.CreateArray
            throw new NotImplementedException();
        }
        else
        {
            var value = moduleRef.AddGlobal(type, node.Name.buffer);
            // var value = builderRef.BuildAlloca(type, node.Name.buffer);
            if (node.AttributesTuple.isExtern)
                value.Linkage = LLVMLinkage.LLVMExternalLinkage;
            CurrentModule.Varaibles.Add(node.Name.buffer, new LLVMVar(value, type));
            {
                if (node.Expression != null)
                {
                    LLVMValueRef eq = node.Expression.Visit(
                        new LLVMExprVisitor(context, builderRef, moduleRef)
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

    public override void Visit(ModuleNode moduleNode)
    {
        CurrentModule = context.SetCurrent(moduleNode.Name.buffer);
        moduleNode.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
    }
    
}