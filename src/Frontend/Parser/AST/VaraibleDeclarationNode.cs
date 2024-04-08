using LLVMSharp.Interop;

public class VaraibleDeclarationNode : INode
{
	public INode? ExpressionNode;
	public LLVMTypeRef typeRef;
	public string name;


	public VaraibleDeclarationNode(LLVMTypeRef type, string name, INode? ExpressionNode)
	{
		this.ExpressionNode = ExpressionNode;
		this.typeRef = type;
		this.name = name;

	}

	// public VaraibleDeclarationNode()
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		LLVMValueRef b;

		if (context.ScopeSize() == 0)
		{
			b = module.AddGlobal(typeRef, name);
			// b = builder.Build(typeRef, name);
			context.AddNewVar(typeRef, name, b);
			if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
			{
				unsafe
				{
					// LLVM.SetLinkage(b, LLVMLinkage.LLVMExternalLinkage);
					LLVM.SetInitializer(b, ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, context)); // Initialize the global variable with value 42
				}
			}
			return b;
		}
		else
		{
			// Console.WriteLine(context.ScopeSize());
			b = builder.BuildAlloca(typeRef, name);
		}
		context.AddNewVar(typeRef, name, b);
		if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
		{

			return builder.BuildStore(ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, context), b);
		}
		return b;
	}
}