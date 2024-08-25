using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMTopLevelVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef) : StatementVisit
{
    
    public override void Visit(PerenNode node)
    {
        Dictionary<string, CompilerModule> modules = new();
        node.ModuleNodes.Values.ToList().ForEach(n =>
        {
            CompilerModule currentModule = new()
            {
                ModuleNode = n
            };
            modules.Add(n.Name.buffer, currentModule);
        });
        modules.Values.ToList()
            .ForEach(n =>
                n.imports = n.ModuleNode.Imports.Select(n1 => { return modules[n1.buffer]; }).ToList());
        LLVMContext context = new(modules);
        node.ModuleNodes.Values.ToList().ForEach(n => n.Visit(new LLVMTypes(builderRef, moduleRef, context)));
        node.ModuleNodes.Values.ToList().ForEach(n => n.Visit(new LLVMFunctions(builderRef, moduleRef, context)));
        node.ModuleNodes.Values.ToList().ForEach(n => n.Visit(new LLVMVaraibles(builderRef, moduleRef, context)));
        node.ModuleNodes.Values.ToList()
            .ForEach(n => { n.Visit(new LLVMStatementVisitor(context, builderRef, moduleRef)); });
    }
}