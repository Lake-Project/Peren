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
		INode f = new FunctionNode("main", new List<INode>{new VaraibleDecleartion("name",new OpNode(new IntegerNode(2),
																				new OpNode(new IntegerNode(10)))),
																				new ReturnStatement(new OpNode(new IntegerNode(2),
																									new OpNode(new IntegerNode(10))))});
		INode f1 = new FunctionNode("test", new List<INode>{new ReturnStatement(new OpNode(new IntegerNode(2),
																new OpNode(new IntegerNode(23))))});
		IRCodeGen.LLVM_Gen(new List<INode> { f, f1 }, "");
	}
}