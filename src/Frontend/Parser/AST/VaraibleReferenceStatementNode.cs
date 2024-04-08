using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : INode
{
	public INode expression;
	public string name;

	public VaraibleReferenceStatementNode(string name, INode expresion)
	{
		this.name = name;
		this.expression = expresion;
	}
	public LLVMValueRef CodeGen(IVisitor visitor, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		unsafe
		{
			Var b = context.GetNewVar(name);
			// var typeRef = LLVM.TypeOf(b);
			// LLVMTypeRef pointedType = LLVM.GetElementType(b.type);
			if (b.type == LLVMTypeRef.Int32)
			{
				return builder.BuildStore(expression.CodeGen(new IntegerExpressionVisitor(), builder, module, context), b.valueRef);

			}
			else
			{

				return builder.BuildStore(expression.CodeGen(new CodeGenVisitor(), builder, module, context), b.valueRef);

			}
		}

		// throw new NotImplementedException();
	}
}