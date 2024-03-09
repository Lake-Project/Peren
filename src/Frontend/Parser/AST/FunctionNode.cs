using LLVMSharp.Interop;

public class FunctionNode : INode
{
	public List<INode> statements;
	string name;
	public FunctionNode(string name, List<INode> statements)
	{
		this.name = name;
		this.statements = statements;
	}
	public LLVMValueRef Accept(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		return Visit(builder, module);
	}

	public LLVMValueRef Visit(LLVMBuilderRef builder, LLVMModuleRef module)
	{
		LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, new LLVMTypeRef[0] { }, false);
		LLVMValueRef function = module.AddFunction(name, funcType);
		LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
		builder.PositionAtEnd(entry);
		foreach (INode statement in statements)
		{
			statement.Accept(builder, module);
		}
		return function;
	}
}