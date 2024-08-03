using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

public class LLVMTopLevelVisitor(LLVMBuilderRef builderRef, LLVMModuleRef moduleRef) : TopLevelVisitor
{
    public override void Visit(VaraibleDeclarationNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(FunctionNode node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(ModuleNode node)
    {
        node.FunctionNodes.ForEach(n => n.Visit(this));
        node.StructNodes.ForEach(n => n.Visit(this));
        node.VaraibleDeclarationNodes.ForEach(n => n.Visit(this));
        node.FunctionNodes.ForEach(n => n.Visit(new LLVMStatementVisitor(builderRef, moduleRef)));
        // node.StatementNodes.ForEach(n => n.Visit(this));
    }

    public override void Visit(TopLevelStatement node)
    {
        throw new NotImplementedException();
    }

    public override void Visit(StructNode node)
    {
        throw new NotImplementedException();
    }
}