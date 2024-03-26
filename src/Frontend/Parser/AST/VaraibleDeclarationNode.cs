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
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{

		LLVMValueRef b = builder.BuildAlloca(typeRef, name);
		scope.AddNewVar(name,b);
		if (typeRef == LLVMTypeRef.Int32)
		{
			
			return builder.BuildStore(ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, ref scope), b);
		}
		return b;
	}
}