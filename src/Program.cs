﻿using LLVMSharp.Interop;
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

		FunctionNode f = new FunctionNode("main", new List<INode>{new VaraibleDeclarationNode(LLVMTypeRef.Int32,"name",new OpNode(
																				new IntegerNode(2),
																				new OpNode(new IntegerNode(2), new IntegerNode(2)))),
																				new ReturnNode(LLVMTypeRef.Int32,
																				new OpNode(new IntegerNode(2),new IntegerNode(10)))
																				});
		// INode f1 = new FunctionNode("test", new List<INode>{new ReturnStatement(new OpNode(new IntegerNode(2),
		// 														new OpNode(new IntegerNode(23))))});

		IRCodeGen.LLVM_Gen(new List<FunctionNode> { f }, "");

	}
}