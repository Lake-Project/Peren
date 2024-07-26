using LacusLLVM.Frontend.Parser.AST;
using Lexxer;
// using Lexxer.Parser;
using LLVMSharp.Interop;

class Program
{
    public static void Main(string[] args)
    {
        // Console.WriteLine(args[0]);
        // int c = 10;
        // const int d = c;
        new CommandLineFlags(args);
        
        

        // new LexTokens();
        // List<Tokens> tokens = new();
        // LexTokens t = new();
        // t.Lex(new string[]
        // {
        //     "1",
        //     "a.b"
        // }, tokens);
        // new CommandLineFlags(new string[] { "test.lk", "--print-ir", "-o", "Test.exe" });
        // List<Tokens> tokens = new LexTokens().Lex(
        //
        // new string[]
        // {
        //
        // "fn factorial(int n) returns int{\n    if(n == 0)\n    {\n        return 1;\n   }\n    return n;\n}\n"
        // //
        // });

        // MonadicParser<INode> p = new (tokens);
        // List<StatementNode> v = p.ParseFile();


        // Console.WriteLine("value: "+v.Visit(new InpretExpr()));
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