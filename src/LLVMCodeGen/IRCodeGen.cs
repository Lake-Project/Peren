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
	public static void LLVM_Gen(List<INode> functions, string file)
	{
		var module = LLVMModuleRef.CreateWithName("main");
		LLVMBuilderRef builder = module.Context.CreateBuilder();
	}
}