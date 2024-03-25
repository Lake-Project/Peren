using LLVMSharp.Interop;

public class VaraibleDeclarationNode : INode
{
	public OpNode ExpressionNode;
	public LLVMTypeRef typeRef;
	public string name;


	public VaraibleDeclarationNode(LLVMTypeRef type, string name, OpNode ExpressionNode)
	{
		this.ExpressionNode = ExpressionNode;
		this.typeRef = type;
		this.name = name;

	}

	// public VaraibleDeclarationNode()
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module)
	{

		LLVMValueRef b = builder.BuildAlloca(typeRef, name);
		if (typeRef == LLVMTypeRef.Int32)
		{
			return builder.BuildStore(ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module), b);
		}
		return b;
	}
}