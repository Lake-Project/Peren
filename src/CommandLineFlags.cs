using System.Collections;
using System.Text.RegularExpressions;
using CommandLine;
using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class CompileOptions
{
    private string _input;

    [Value(
        index: 0,
        MetaName = "inputFile",
        HelpText = "The Lacus source file to compile",
        Required = true
    )]
    public IEnumerable<string> InputFiles { get; set; }

    public LLVMCodeGenOptLevel OptLevel;

    [Option('O', "optimize", Required = false, HelpText = "Set optimization level.")]
    public string? OptimizationLevels
    {
        get { return null; }
        set
        {
            if (value.Equals("1"))
                OptLevel = LLVMCodeGenOptLevel.LLVMCodeGenLevelLess;
            else if (value.Equals("2"))
                OptLevel = LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault;
            else if (value.Equals("3"))
                OptLevel = LLVMCodeGenOptLevel.LLVMCodeGenLevelAggressive;
            else if (value.Equals("0"))
                OptLevel = LLVMCodeGenOptLevel.LLVMCodeGenLevelLess;
        }
    }

    [Option('c', Default = false)] public bool CompileOnly { get; set; }

    [Option("emit-ir", Required = false, Default = false, HelpText = "saves ir to file")]
    public bool IrFile { get; set; }

    [Option("print-ir", Required = false, Default = false, HelpText = "prints IR to console")]
    public bool PrintIR { get; set; }

    [Option("dump-tokens", HelpText = "prints Tokens")]
    public bool PrintTokens { get; set; }

    [Option('o', "output", Default = "a.exe", HelpText = "output file")]
    public string OutputFile { get; set; }

    [Option('a', "assembly", HelpText = "emit assembly file")]
    public bool AssemblyFile { get; set; }

    [Option('S', "compile-off", Default = false, HelpText = "doesnt link or emit an obj ")]
    public bool CompileOff { get; set; }

    [Option(
        "target",
        Default = "generic",
        HelpText = "target cpu (run clang -print-supported-cpus to see list"
    )]
    public string targetArchitechure { get; set; }
}

public class CommandLineFlags
{
    private string[] _args { get; }

    public CommandLineFlags(string[] args)
    {
        _args = args;
        // new Options().InputFiles = "a";
        Parser
            .Default.ParseArguments<CompileOptions>(args)
            .WithParsed<CompileOptions>(options => RunCompiler(options))
            .WithNotParsed(errors => Console.WriteLine($"error running lakec"));
    }

    public void RunCompiler(CompileOptions compileOptions)
    {
        // Console.WriteLine($"-c: {compileOptions.ObjFile}");
        // List<string> files = compileOptions.InputFiles.
        // List<Tokens> tokens = new LexTokens().Lex(File.ReadAllLines(compileOptions.InputFiles));
        // [..compileOptions.InputFiles.ToList().Map
        List<Tokens> tokens = new();
        LexTokens t = new();
        compileOptions.InputFiles.ToList().ForEach( n =>
        {
            t.Lex(File.ReadAllLines(n), tokens);
        });
        if (compileOptions.PrintTokens)
            tokens.ForEach(n => Console.WriteLine(n.ToString()));

        // Parse p = new MonadicParser<INode>(tokens);
        List<StatementNode> s = new Parse(tokens).ParseFile();
        new SemanticAnaylsis().SemanticEntry(s);
        IRCodeGen.LLVM_Gen(s, compileOptions);
    }
}