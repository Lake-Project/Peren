using LLVMSharp.Interop;

public interface IExpression
{
	// public IExpression(OpNode expression);
	public LLVMValueRef Solve(OpNode op, LLVMBuilderRef builder, LLVMModuleRef moduleRef);
	// public LLVMValueRef Visit(INode node, LLVMBuilderRef builderRef, LLVMModuleRef moduleRef);
}