using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor.Backend;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMFunctions(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef, LLVMContext context) : StatementVisit
{
    public CompilerModule CurrentModule = new();

    public override void Visit(ModuleNode moduleNode)
    {
        CurrentModule = context.SetCurrent(moduleNode.Name.buffer);

        moduleNode.FunctionNodes.ForEach(n => n.Visit(this));
    }


    public override void Visit(FunctionNode node)
    {
        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(
            Compile.ToLLVMType(node.RetType.Name, context),
            node.Parameters //params
                .Select(n => Compile.ToLLVMType(n.Type, context)) //converts param types
                .ToArray()
        );
        LLVMValueRef function = moduleRef.AddFunction(node.Name.buffer, funcType);
        function.Linkage = LLVMLinkage.LLVMExternalLinkage;
        CurrentModule.Functions.Add(node.Name.buffer,
            new LLVMFunction(funcType, Compile.ToLLVMType(node.RetType.Name, context), function));
    }
}