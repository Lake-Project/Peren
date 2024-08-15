using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMTypes(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef, LLVMContext context) : StatementVisit
{
    public CompilerModule CurrentModule = new();

    public override void Visit(StructNode node)
    {
        var llvmstruct = moduleRef.Context.CreateNamedStruct(node.Name.buffer);
        CurrentModule.Types.Add(node.Name.buffer, new LLVMType(
            llvmstruct, node.Vars));
    }

    public override void Visit(ModuleNode moduleNode)
    {
        moduleNode.StructNodes.ForEach(n => n.Visit(this));
        // base.Visit(moduleNode);
    }

    public override void Visit(PerenNode node)
    {
        node.ModuleNodes.Values.ToList().ForEach(n =>
        {
            CurrentModule = context.SetCurrent(n.Name.buffer);
            n.Visit(this);
        });
    }
}