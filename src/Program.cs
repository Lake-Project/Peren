using LLVMSharp.Interop;

class Program
{
    public static void Main(string[] args)
    {
        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllAsmPrinters();
        LLVM.InitializeAllAsmParsers();
        // Console.WriteLine(args[0]);
        new CommandLineFlags(args);
        // new CommandLineFlags(new string[] { "test.lk", "--print-ir", "-o", "Test.exe" });
        // List<Tokens> tokens = new LexTokens().Lex(File.ReadAllLines("test.lk"));
        // // new LexTokens().printList(tokens);
        // Parse p = new Parse(tokens);
        // List<INode?> d = p.ParseFile();
        // fs += 10;
        // char f = 1;
        // IRCodeGen.LLVM_Gen(d, "");

        // VaraibleDeclarationNode v = new(LLVMTypeRef.Int32, "global_var", new OpNode(new IntegerNode(2), new IntegerNode(2)));
        // FunctionNode f = new("testLink", LLVMTypeRef.Int32, new List<INode>{new VaraibleDeclarationNode(LLVMTypeRef.Int32,"name",new OpNode(
        // 																		new IntegerNode(2),
        // 																		new OpNode(new IntegerNode(2), new IntegerNode(2)))),
        // 																		new VaraibleReferenceStatementNode("name",
        // 																		new OpNode(new IntegerNode(2),null)),
        // 																		new VaraibleReferenceStatementNode("global_var",new OpNode(new IntegerNode(100),null)),
        // 																		new ReturnNode(LLVMTypeRef.Int32,
        // 																		new OpNode(new VaraibleReferenceNode("global_var"),new IntegerNode(1)))
        // 																		});
        // IRCodeGen.LLVM_Gen(new List<INode> { v, f }, "");
    }
}
