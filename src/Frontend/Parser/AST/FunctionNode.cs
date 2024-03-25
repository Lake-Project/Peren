using LLVMSharp.Interop;

public class FunctionNode : INode
{

	public List<INode> statements;
	public string name;
	public FunctionNode(string name, List<INode> statements)
	{
		this.name = name;
		this.statements = statements;
	}
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module)
	{

		// return visitor.visit(this, builder, module);
		LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, new LLVMTypeRef[0] { }, false);
		LLVMValueRef function = module.AddFunction(name, funcType);
		LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
		builder.PositionAtEnd(entry);
		for (int i = 0; i < statements.Count; i++)
		{
			statements[i].CodeGen(visitor, builder, module);
		}
		return function;

	}
}