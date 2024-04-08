using LLVMSharp.Interop;
using Lexxer;
public class IntegerExpressionVisitor : IVisitor
{
	public LLVMValueRef Visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		unsafe
		{
			return LLVM.ConstInt(LLVM.Int32Type(), (ulong)node.n, 1);

		}
	}

	public LLVMValueRef Visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		throw new TypeMisMatchException();
	}

	public LLVMValueRef Visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		if (node.left == null && node.right != null)
		{
			return node.right.CodeGen(this, builder, module, context);
		}
		else if (node.right == null && node.left != null)
		{
			return node.left.CodeGen(this, builder, module, context);
		}
		else if (node.right == null && node.left == null)
		{
			unsafe
			{
				return LLVM.ConstInt(LLVM.Int32Type(), (ulong)0, 1);

			}
		}
		else
		{
#pragma warning disable CS8602 // Dereference of a possibly null reference.

			return builder.BuildAdd(node.right.CodeGen(this, builder, module, context),
			node.left.CodeGen(this, builder, module, context), "addtmp");
			// return node.token.tokenType switch
			// {
			// 	TokenType.ADDITION => builder.BuildAdd(opNode.Left.CodeGen(this,builder, module), Right.Accept(builder, module), "addtmp"),
			// 	TokenType.SUBTRACTION => builder.BuildSub(Left.Accept(builder, module), Right.Accept(builder, module), "subtmp"),
			// 	TokenType.DIVISION => builder.BuildSDiv(Left.Accept(builder, module), Right.Accept(builder, module), "divtmp"),
			// 	TokenType.MULTIPLICATION => builder.BuildMul(Left.Accept(builder, module), Right.Accept(builder, module), "multmp"),
			// 	TokenType.MODULAS => builder.BuildSRem(Left.Accept(builder, module), Right.Accept(builder, module), "modtmp"),
			// 	_ => builder.BuildAdd(Left.Accept(builder, module), Right.Accept(builder, module), "addtmp")
			// };
		}
	}

	public LLVMValueRef Visit(VaraibleReferenceNode node, LLVMBuilderRef builder, LLVMModuleRef module, Context context)
	{
		// throw new NotImplementedException();
		Var l = context.GetNewVar(node.name);
		return builder.BuildLoad2(l.type, l.valueRef);
	}
}