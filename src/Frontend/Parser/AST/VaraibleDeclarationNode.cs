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
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Scope scope)
	{
		LLVMValueRef b;

		if (scope.ScopeSize() == 0)
		{
			b = module.AddGlobal(typeRef, name);
			// b = builder.Build(typeRef, name);
			scope.AddNewVar(typeRef, name, b);
			if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
			{
				unsafe
				{
					// LLVM.SetLinkage(b, LLVMLinkage.LLVMExternalLinkage);
					LLVM.SetInitializer(b, ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, scope)); // Initialize the global variable with value 42
				}
			}
			return b;
		}
		else
		{
			Console.WriteLine(scope.ScopeSize());
			b = builder.BuildAlloca(typeRef, name);
		}
		scope.AddNewVar(typeRef, name, b);
		if (typeRef == LLVMTypeRef.Int32 && ExpressionNode != null)
		{

			return builder.BuildStore(ExpressionNode.CodeGen(new IntegerExpressionVisitor(), builder, module, scope), b);
		}
		return b;
	}
}