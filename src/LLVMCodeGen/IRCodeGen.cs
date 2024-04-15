using LLVMSharp;
using LLVMSharp.Interop;
public class IRCodeGen
{

	// private static LLVMValueRef IntegerExpression(LLVMBuilderRef builder, LLVMModuleRef module)
	// {

	// }
	private static void LLVMCreateFile(LLVMModuleRef module, string file)
	{
		// Specify the directory path
		string directoryPath = "out";
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
		if (file == "")
		{
			file = "output.ll";
		}
		string filePath = Path.Combine(directoryPath, file);
		File.WriteAllText(filePath, module.ToString());
		Console.WriteLine("code successfully compiled");
		module.Dispose();

	}
	public static void LLVM_Gen(List<INode?> statements, string file)
	{
		var module = LLVMModuleRef.CreateWithName("main");
		LLVMBuilderRef builder = module.Context.CreateBuilder();
		Context c = new Context();
		// Context context = new CO
		foreach (INode? statement in statements)
			statement.CodeGen(new CodeGenVisitor(), builder, module, c);
		string directoryPath = "out";
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
		if (file == "")
		{
			file = "output.ll";
		}
		string filePath = Path.Combine(directoryPath, file);
		File.WriteAllText(filePath, module.ToString());
		Console.WriteLine("code successfully compiled");
		Console.WriteLine("IR code gen file path: " + filePath);

		// new FloatNode().Accept<FloatNode>(new FloatExprVis(), builder, module);
		// LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, new LLVMTypeRef[0] { }, false);
		// LLVMValueRef function = module.AddFunction("main", funcType);
		// LLVMBasicBlockRef entry = function.AppendBasicBlock("entry");
		// builder.PositionAtEnd(entry);
		// builder.BuildRet(expr.Accept(builder, module));
		// Console.WriteLine(module.ToString());

	}
}