using CommandLine;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;


public enum OptLevel
{
    Level0,
    Level1,
    Level2,
    Level3
}

public class CompileOptions
{
    private string _input;


    [Value(
        index: 0,
        MetaName = "inputFile",
        HelpText = "a peren source file to compile",
        Required = true
    )]
    public IEnumerable<string> InputFiles { get; set; }

    public OptLevel OptLevel { get; set; }

    [Option('O', "optimize", Required = false, HelpText = "Set optimization level.")]
    public string? OptimizationLevels
    {
        set
        {
            OptLevel = value switch
            {
                "0" => OptLevel.Level0,
                "1" => OptLevel.Level1,
                "2" => OptLevel.Level2,
                "3" => OptLevel.Level3,
                _ => OptLevel.Level3
            };
        }
    }

    [Option('c', Default = false)] public bool CompileOnly { get; set; }

    [Option("emit-ir", Required = false, Default = false, HelpText = "saves ir to file")]
    public bool IrFile { get; set; }

    [Option("print-ir", Required = false, Default = false, HelpText = "prints IR to console")]
    public bool PrintIR { get; set; }

    [Option("dump-tokens", HelpText = "prints Tokens")]
    public bool PrintTokens { get; set; }

    [Option('o', "output", Default = "a.out", HelpText = "output file")]
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
    public string TargetArchitechure { get; set; }
    
    [Option(
        "linker",
        Default = "clang",
        HelpText = "Linker you want to use for compiling"
    )]
    public string Linker { get; set; }
}

public class Perenc
{
    public static void Init(string[] args)
    {
        Parser
            .Default.ParseArguments<CompileOptions>(args)
            .WithParsed(options => RunCompiler(options))
            .WithNotParsed(errors =>
            {
                using var enumerator = errors.GetEnumerator();
                enumerator.MoveNext();
                Console.WriteLine($"error invoking perenc {enumerator.Current}, " +
                                  $"run perenc --help to see a list of options");
            });
    }

    private static void RunCompiler(CompileOptions compileOptions)
    {
        List<Tokens> tokens = new();

        compileOptions.InputFiles.ToList()
            .SelectMany(inputDirOrFile =>
                Directory.Exists(inputDirOrFile)
                    ? Directory.GetFiles(inputDirOrFile, "*.pn")
                    : [inputDirOrFile])
            .ToList()
            .ForEach(inputFile =>
                new LexTokens().LexList(File.ReadAllLines(inputFile),
                    tokens)); //little function designed to grab All the files in a Directory and lexes them:3
        List<int> a = new List<int>() { 1, 2, 3, 4 };
        // a.ForEach(Console.WriteLine);
        if (compileOptions.PrintTokens)
            tokens.ForEach( token => Console.WriteLine(token)); //prints the tokens

        var parsedProgram = new Parse(tokens).ParseFile();

        SemanticAnaylsis.Init(parsedProgram);
        IRCodeGen.LLVM_Gen(parsedProgram, compileOptions);
    }
}