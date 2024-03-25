using LLVMSharp.Interop;
using Lexxer;
public class IntegerExpressionVisitor : IVisitor
{
	public LLVMValueRef visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		unsafe
		{
			return LLVM.ConstInt(LLVM.Int32Type(), (ulong)node.n, 1);

		}
	}

	public LLVMValueRef visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		throw new TypeMisMatchException();
	}

	public LLVMValueRef visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module)
	{
		if (node.left == null && node.right != null)
		{
			return node.right.CodeGen(this, builder, module);
		}
		else if (node.right == null && node.left != null)
		{
			return node.left.CodeGen(this, builder, module);
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
			return builder.BuildAdd(node.right.CodeGen(this, builder, module), node.left.CodeGen(this, builder, module), "addtmp");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
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
}