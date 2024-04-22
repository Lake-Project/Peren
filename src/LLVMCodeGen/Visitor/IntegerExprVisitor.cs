using LLVMSharp.Interop;
using Lexxer;

public class IntegerExpressionVisitor : IVisitor
{
	private HashSet<LLVMTypeRef> AllowedTypes = new HashSet<LLVMTypeRef> { LLVMTypeRef.Int32 };

	public IntegerExpressionVisitor() { }

	public LLVMValueRef Visit(
		IntegerNode node,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
	{
		unsafe
		{
			return LLVM.ConstInt(LLVM.Int32Type(), (ulong)node.n, 1);
		}
	}

	public LLVMValueRef Visit(
		FloatNode node,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
	{
		throw new TypeMisMatchException();
	}

	public LLVMValueRef Visit(
		OpNode node,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
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

			// return builder.BuildAdd(
			//     node.right.CodeGen(this, builder, module, context),
			//     node.left.CodeGen(this, builder, module, context),
			//     "addtmp"
			// );
			return node.token.tokenType switch
			{
				TokenType.ADDITION
				  => builder.BuildAdd(
					  node.right.CodeGen(this, builder, module, context),
					  node.left.CodeGen(this, builder, module, context),
					  "addtmp"
				  ),
				TokenType.SUBTRACTION
				  => builder.BuildSub(
					  node.right.CodeGen(this, builder, module, context),
					  node.left.CodeGen(this, builder, module, context),
					  "subtmp"
				  ),
				TokenType.MULTIPLICATION
				  => builder.BuildMul(
					  node.right.CodeGen(this, builder, module, context),
					  node.left.CodeGen(this, builder, module, context),
					  "multmp"
				  ),
				TokenType.DIVISION
				  => builder.BuildSDiv(
					  node.right.CodeGen(this, builder, module, context),
					  node.left.CodeGen(this, builder, module, context),
					  "divtmp"
				  ),
				TokenType.MODULAS
				  => builder.BuildURem(
					  node.right.CodeGen(this, builder, module, context),
					  node.left.CodeGen(this, builder, module, context),
					  "modtmp"
				  ),
				_ => throw new Exception("unsupported op")
			};
		}
	}

	public LLVMValueRef Visit(
		VaraibleReferenceNode node,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
	{
		// throw new NotImplementedException();
		Var l = context.GetVar(node.name);
		return builder.BuildLoad2(l.type, l.valueRef);
	}

	public LLVMValueRef Visit(
		FunctionCallNode node,
		LLVMBuilderRef builder,
		LLVMModuleRef module,
		Context context
	)
	{
		Function fun = context.GetFunction(node.Name);
		if (fun.f.retType == LLVMTypeRef.Void)
			throw new Exception("");
		return builder.BuildCall2(fun.type, fun.ValueRef, node.Values, fun.f.name);
	}
}
