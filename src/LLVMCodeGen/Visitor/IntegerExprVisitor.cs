using LLVMSharp.Interop;
using Lexxer;
public class IntegerExpressionVisitor : IVisitor
{
	public LLVMValueRef Visit(IntegerNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{
		unsafe
		{
			return LLVM.ConstInt(LLVM.Int32Type(), (ulong)node.n, 1);

		}
	}

	public LLVMValueRef Visit(FloatNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{
		throw new TypeMisMatchException();
	}

	public LLVMValueRef Visit(OpNode node, LLVMBuilderRef builder, LLVMModuleRef module, ref Scope scope)
	{
		if (node.left == null && node.right != null)
		{
			return node.right.CodeGen(this, builder, module, ref scope);
		}
		else if (node.right == null && node.left != null)
		{
			return node.left.CodeGen(this, builder, module, ref scope);
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

			return builder.BuildAdd(node.right.CodeGen(this, builder, module, ref scope),
			node.left.CodeGen(this, builder, module, ref scope), "addtmp");
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