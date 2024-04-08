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
		// new LexTokens().printList(tokens);
		// Parse p = new Parse(tokens);
		// IRCodeGen.LLVM_Gen(p.ParseFile(), "");
		VaraibleDeclarationNode v = new VaraibleDeclarationNode(LLVMTypeRef.Int32, "global_var", new OpNode(new IntegerNode(2), new IntegerNode(2)));
		FunctionNode f = new FunctionNode("testLink", LLVMTypeRef.Void, new List<INode>{new VaraibleDeclarationNode(LLVMTypeRef.Int32,"name",new OpNode(
																				new IntegerNode(2),
																				new OpNode(new IntegerNode(2), new IntegerNode(2)))),
																				new VaraibleReferenceStatementNode("name",
																				new OpNode(new IntegerNode(2),null)),
																				new VaraibleReferenceStatementNode("global_var",new OpNode(new IntegerNode(100),null)),
																				new ReturnNode(LLVMTypeRef.Int32,
																				new OpNode(new VaraibleReferenceNode("global_var"),new IntegerNode(1)))
																				});
		// INode f1 = new FunctionNode("test", new List<INode>{new ReturnNode(new OpNode(new IntegerNode(2),
		// new OpNode(new IntegerNode(23),null)))});

		IRCodeGen.LLVM_Gen(new List<INode> { v, f }, "");

	}
}