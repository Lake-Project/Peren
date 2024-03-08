using LLVMSharp.Interop;
using Lexxer;
class Program
{
	public static void Main()
	{
		LLVM.InitializeAllTargetInfos();
		LLVM.InitializeAllTargets();
		LLVM.InitializeAllTargetMCs();
		LLVM.InitializeAllAsmPrinters();
		LLVM.InitializeAllAsmParsers();
		// List<Tokens> tokens = new LexTokens().Lex(File.ReadAllLines("test.lk"));
		IRCodeGen.LLVM_Gen(new OpNode(new IntegerNode(2),
						new OpNode(new IntegerNode(10))), "");
	}
}